# 现代 .NET 迁移后的 fuzzing 验证

## 端到端 TCP 场景

新增样例 [ModernTcpFuzz.xml](../pro/Peach/samples/ModernTcpFuzz.xml) 使用现代 `Tcp` 发布器和一个带结构关系的帧协议：魔数、版本、标志位、`size` 关系、命令/主题/JSON 二进制正文以及 CRC32 fixup。

使用一个独立的本地 TCP sink 作为真实对端，执行：

```text
Peach.dll --noweb --polite --range=1,1000 --seed=424242 \
  -DTargetHost=127.0.0.1 -DTargetPort=44243 \
  pro/Peach/samples/ModernTcpFuzz.xml
```

结果：1,000/1,000 轮完成，无 Peach fault；对端确认收到 1,000 个连接。实际帧大小覆盖 2 到 2,117 字节，包含 `NumberVariance`、`NumberEdgeCase`、`DataElementRemove`、`DataElementDuplicate`、`DataElementSwapNear`、Unicode/UTF-8 变异、Blob 扩展/缩减、CRC 字段变异等。总耗时约 53.39 秒，约 18.7 iter/s。

同一场景接入原有 `CrashableServer` 时，在第 273 轮重现了它设计好的 1,024 字节崩溃阈值；Peach 正确报告并停止在 control iteration。这验证了迁移后的 fault 检测和重现流程，不属于 Peach 自身崩溃。

## 发布器测试

- File、FilePerIteration、Zip：9/9 通过。
- TcpClient、TcpListener：17/17 通过。
- HTTP：8/8 通过，另有 1 个原测试因证书 URL 保留而跳过。迁移期间修复了 .NET 8 的连接异常重试识别和 `Stream.CopyTo` 从当前位置复制导致 POST 空载荷的问题。
- SSL：两个测试均为原有 `[Ignore]`（一个尚未实现，一个要求外部 OpenSSL 服务）。
- WebSocket：已改用 `HttpListener` + `System.Net.WebSockets`；2/2 测试通过，其中包含真实 `ClientWebSocket` 的 ready、Base64 模板和 evaluation-complete 往返。

## 全面回归验证

以下结果是在当前 Linux 沙箱中重新执行的，`--no-build` 测试使用同一次 Release 构建产物：

- `dotnet build Peach.Build.csproj --no-restore`：0 errors，96 warnings。警告主要是旧 .NET Framework 资产的 `NU1701`、DotNetZip/SSH.NET 的安全公告 `NU1903/NU1902`，以及 `System.*` 程序集冲突 `MSB3243`；它们没有阻止生成，但说明依赖还没有完全现代化。
- Core：138 passed，1 skipped（`EnsureSerializable`）。
- Storage + JobRunner：27/27 passed（Database 19、JobRunner 8）。为降低现代 SQLite 并发锁冲突，数据库连接字符串增加了 busy timeout/default timeout；测试资源和 SQL 字符串也按现代 SQLite 语义补齐。
- JobLogger：11/11 passed。重放跳转前的 health-check control iteration（`* C 1`）已作为引擎既有语义纳入验证。
- 不依赖特权网络的 File、FilePerIteration、Zip、Tcp、TcpListener、HTTP：30 passed，1 skipped。
- ProcessMonitor + RunCommand：38/38 passed。补齐跨平台进程夹具，并修正 `StartOnCall`、每轮重启和 iteration-finished 的退出判定；ASAN 文本解析路径经过 30 次重复测试。
- REST Agent：13/15 passed。monitor/publisher HTTP RPC、重连、fault data 和以 `System.Net.WebSockets` 实现的远程日志均通过；2 项失败引用仓库不存在的 `TcpPort` monitor 与 `Rest` publisher。命令行 `PeachAgent` 加真实远程 UDP publisher 端到端通过。
- `PitTool` 的 XML/StringToken/Regex analyzer、`--help`、HelloWorld/Relation CLI 样例均可启动；`ModernTcpFuzz.xml` 通过 PitLint。

### 已复现但尚未修复的问题

1. **仓库缺少部分插件源码/资源**：全量 Pro 基线为 1381 passed、94 failed、27 skipped（启用新迁移的 15 个 REST 测试之前）。主要失败来自不存在的 `TcpPort`、`Memory`、`Rest`、ASN.1/JSON、DNP3、crypt transformer 和 Titanium Web Proxy，而不是可通过 target-framework 修改解决的代码。
2. **旧 PIT schema/fixture 仍有缺口**：部分测试仍使用旧 `Action type='message'`/`outfrag` 语法，或引用未入库的 DNP3 manifest。PitCompiler lint 还有文件名前缀和 system define 数量的旧基线差异。
3. **任意回调的强制中断无法等价**：.NET 8 不支持 `Thread.Abort`。引擎已采用协作取消，但无法强行中断第三方事件处理器中不观察取消信号的 `Sleep`，因此 duration-abort 的旧测试语义无法完全保持。
4. **原生崩溃夹具语义尚未等价**：新增的 .NET `CrashingFileConsumer` 保留文件读取/快速退出行为；`UseAfterFree` 输出规范 ASAN 文本以验证解析链路，但二者都不替代真实 native sanitizer/core-dump 集成测试。

### 当前环境导致的失败（不应直接归因于 .NET 迁移）

- Linux OS 测试为 6 passed、19 failed、1 skipped。GDB 用例缺少 `gdb/exploitable/exploitable.py`；LinuxCoreFile 用例没有权限写 `/proc/sys/kernel/core_pattern`；`TestCpuUsage` 假定主机名为 `init`，沙箱实际为 `codex-linux-sandbox`。
- UDP/raw socket 和 MTU 用例需要 `CAP_NET_RAW`/网卡管理权限；当前容器返回 `Operation not permitted`。`UdpNoPortRecv` 还会因无法建立对应网络场景而超时。SSL 测试仍是原有 `[Ignore]`，一个未完成，一个要求外部 OpenSSL 服务。
- 进程测试中的 `TestStartOnCall` 对 1.9 秒等待窗口的断言与当前 `MonitorRunner` 的回调顺序不一致；它测到的是 `IterationFinished` 回调本身而不是随后 `DetectedFault` 的等待。需要在专用环境确认旧实现是否也依赖这一时序。

## 结论与后续优先级

核心引擎、File/TCP/HTTP/WebSocket、现代 Agent/远程 UDP、进程监视与长时间 fuzzing 已在现代 .NET 下工作；构建和核心测试保持通过。下一步优先取得或重写缺失的商业插件/PIT 资产，并在专用 CI 中验证 Linux 特权、GDB、真实 core dump/ASAN、SSL、COM、legacy Remoting 和 Windows GUI。
