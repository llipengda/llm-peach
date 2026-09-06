# .NET 现代化迁移说明

本文说明 `codex/modern-dotnet-fuzzing` 分支中的迁移内容。目标是把原来依赖 Waf/Paket/Docker 和 .NET Framework 的项目，逐步迁移到 .NET 8，同时保持源码目录结构不变。

## 总体结果

- 在 `core/`、`pro/`、`llm/` 和 `3rdParty/BouncyCastle/` 原目录旁新增 SDK-style `.csproj`。
- 新增根级聚合项目 [Peach.Build.csproj](../Peach.Build.csproj)，用于一次构建可移植的 Core、Pro、命令行工具、测试、LLM 验证组件和 Bouncy Castle。
- 删除了临时的 `modern/` 镜像目录，避免仓库同时存在两套源码结构。
- 默认目标框架为 `net8.0`；WinForms 工具使用 `net8.0-windows`，并只在 Windows SDK 环境下纳入聚合构建。
- 原有 .NET Framework/Waf 项目仍保留，便于对照和处理尚未重写的功能。

## 新增文件

### 构建与运行时基础

- `global.json`：固定 .NET SDK 主版本为 .NET 8，并允许使用最新 8.0 补丁版本。
- `Peach.Build.csproj`：根级聚合构建入口，引用所有已迁移的项目。
- `core/Core/Peach.Core.csproj`：Core 主程序集的 .NET 8 项目定义。
- `core/Core/ModernRuntime.cs`：通过模块初始化注册代码页编码，保证旧编码名称在 .NET 8 中仍可用。
- `core/Minset/PeachMinset.csproj`：Minset 命令行工具项目。
- `3rdParty/BouncyCastle/crypto/BouncyCastle.Crypto.csproj`：将仓库内 Bouncy Castle 源码直接编译为 .NET 8 程序集。

### Pro 和命令行工具

- `pro/Core/Peach.Pro.csproj`：Pro 核心程序集，引用 Core、Bouncy Castle 和所需 NuGet 包。
- `pro/Peach/Peach.csproj`：Peach 主命令行程序。
- `pro/PeachAgent/PeachAgent.csproj`：Agent 命令行程序。
- `pro/Worker/PeachWorker.csproj`：Worker 程序。
- `pro/Service/PeachService.csproj`：Service 程序的无 Web UI 运行入口。
- `pro/PitTester/Peach.Pro.PitTester.csproj`：PIT 测试工具。
- `pro/PitTool/PitTool.csproj`：PIT 工具。
- `pro/OS/UnixTrampoline/PeachTrampoline.csproj`：Unix trampoline 的 .NET 8 项目定义；旧 IPC 模式仍不支持。

### LLM 验证组件

以下项目把原来位于 `ide/projects/` 逻辑上的组件放回对应的 `llm/` 源码目录：

- `llm/Core/Peach.LLM.csproj`
- `llm/Validations/Common/Peach.LLM.Validations.Common.csproj`
- `llm/Validations/DataModel/Peach.LLM.Validations.DataModel.csproj`
- `llm/Validations/Mutator/Peach.LLM.Validations.Mutator.csproj`
- `llm/Validations/Fixer/Peach.LLM.Validations.Fixer.csproj`

### 测试项目和测试辅助程序

- `core/Test/Peach.Core.Test.csproj`：Core 测试迁移到现代 NUnit、Microsoft.NET.Test.Sdk 和 .NET 8。
- `pro/Test/Core/Peach.Pro.Test.csproj`：Pro 测试迁移到 .NET 8，并引用迁移后的测试工具。
- `pro/Test/OS/Linux/Peach.Pro.Test.OS.Linux.csproj`：Linux 平台测试项目。
- `pro/MutatorsTest/Peach.Pro.MutatorsTest.csproj`：修复为空测试壳项目时的现代构建设置。
- `pro/Test/Apps/CrashTest/CrashTest.csproj`、`Program.cs`：用 .NET 8 apphost 重写进程、超时、正则输出和信号测试工具。
- `pro/Test/Apps/CrashableServer/CrashableServer.csproj`、`Program.cs`：用 .NET 8 重写 TCP 回显/超时/故障测试服务。
- `pro/Test/Apps/CrashingFileConsumer/CrashingFileConsumer.csproj`、`Program.cs`：替代原生 C++/waf 文件消费者，保持进程监视器所需的命令行/快速退出行为；不会刻意复现原生缓冲区越界，真实 core dump/ASAN 仍需原生测试资产。
- `pro/Test/Apps/UseAfterFree/UseAfterFree.csproj`、`Program.cs`：生成稳定的 ASAN 文本夹具，用来验证 ProcessMonitor 的 sanitizer 解析、hash 和 fault 数据链路；它不是原生内存破坏程序。
- `pro/Test/Apps/CrashTestDummy/CrashTestDummy.csproj`、`Program.Modern.cs`：保持原目录，提供跨平台的单实例/canary 测试程序，替代只支持 .NET Framework WinForms 的旧入口。
- `pro/Core/Agent/Channels/Rest/ModernLogChannel.cs`：使用 `System.Net.WebSockets` 重写 Agent 远程日志通道，并以显式 DTO 取代对 NLog 内部对象图的序列化。

### 文档

- `docs/modern-dotnet.adoc`：英文构建说明、迁移范围和兼容性边界。
- `docs/modern-dotnet-migration.zh-CN.md`：本文，说明具体文件和改动原因。
- `README.adoc`：增加现代 .NET 构建说明的入口链接。

## 修改文件及原因

### Core 运行时和兼容性

- `core/Core/Engine.cs`：将 .NET Framework 下的 `Thread.Abort` 路径改为 .NET 8 下的协作式停止；保留旧目标框架的条件编译分支。
- `core/Core/IO/NonClosingStream.cs`：为现代 .NET 调整流关闭和异步 API 的兼容实现。
- `core/Core/ObjectCopier.cs`：补充现代运行时缺少 BinaryFormatter 序列化标记时的克隆路径，包括 `Encoding`、比较器、`IPAddress` 和泛型 `Dictionary`。
- `core/Core/Utilities.cs`：将调试 Trace listener 的注册改为 .NET 8 可用的条件实现。
- `core/Test/BitwiseStreamTest.cs`、`core/Test/EncodingTests.cs`、`core/Test/UtilitiesTests.cs`：更新 .NET 8 下异常、编码和断言行为的测试写法。
- `core/Test/TestBase.cs`：让 NUnit 测试日志断言在 .NET 8 使用 `Trace` 监听器。

### Pro 运行时和平台适配

- `pro/Core/OS/Unix/ProcessImpl.cs`：移除 Unix 进程启动对 Mono debugger-agent 和旧 Peach trampoline 协议的依赖，改用 `System.Diagnostics.Process` 直接启动，并用 POSIX 信号和 .NET 进程树终止。
- `pro/Core/Runtime/JobRunner.cs`：取消 PIT 解析阶段的线程强杀，改用停止标记、事件唤醒和引擎协作式取消。
- `pro/Core/Loggers/JobLogger.cs`、`pro/Core/Loggers/DatabaseTarget.cs`：兼容现代 NLog/SQLite 的启动时序；Job parent 尚未创建时跳过孤立启动日志，并在测试 license 缺失时避免空引用。
- `pro/Core/Storage/Database.cs`：为 WAL 连接显式设置 busy/default timeout，降低 .NET 8 下并发日志写入的锁冲突。
- `pro/Core/Publishers/SslClientPublisher.cs`：TLS 异步读取改为后台线程加流关闭唤醒，不再调用 `Thread.Abort`。
- `pro/Core/Publishers/WebSocketPublisher.cs`：以 `HttpListener` 和 `System.Net.WebSockets` 替换 vtortola/WebSocketSharp，保留浏览器 ready、模板传输和 evaluation complete 协议。
- `pro/Core/Agent/Channels/Rest/Listener.cs`、`Extensions.cs`、`RouteHandler.cs`、`RouteResponse.cs`、`MonitorHandler.cs`、`PublisherHandler.cs`：将旧 SocketHttpListener Agent 服务迁至 .NET 8 自带 HTTP/WebSocket 栈，恢复 `tcp://` monitor、远程 publisher、重连和日志转发。
- `pro/Core/PitResource.cs`、`pro/PitTester/PitTester.cs`：用 Mono.Cecil 替换 .NET Framework 独有的可保存 Reflection.Emit assembly API。
- `pro/Core/WebServices/ExternalJobMonitor.cs`：按平台启动现代 `PeachWorker` apphost，并修复未启动任务的释放路径。
- `pro/Core/Analyzers/ZipAnalyzer.cs`、`pro/Core/Publishers/ZipPublisher.cs`、`pro/Core/Transformers/Compress/Bz2Compress.cs`、`Bz2Decompress.cs`：为 DotNetZip 外部程序集增加 alias，解决与其他压缩库的类型冲突。
- `pro/Core/Mutators/StringXmlW3C.cs`：改用现代 `System.IO.Compression` ZIP API。
- `pro/Core/Mutators/DataElementDuplicate.cs`：移除无法在现代编译器下使用的旧成员引用。
- `pro/Peach/PeachMain.cs`、`pro/Service/ServiceMain.cs`：移除经典 WebApi2 注入，支持无 Web UI 的 CLI/Service 启动。
- `pro/OS/UnixTrampoline/TrampolineMain.cs`：保留 Unix 执行能力；旧 .NET Remoting `--ipc` 模式改为明确报告“不支持”，避免运行时崩溃。

### 测试项目配置和测试数据

- `pro/Test/Core/Agent/LegacyMonitorTests.cs`：使用现代 NUnit 断言 API。
- `pro/Test/Core/Dom/VarNumberTests.cs`：将已不存在的 `outfrag` action 改为当前 schema 支持的 `output` action；该测试实际验证的是 VarNumber 长度和值。
- `pro/MutatorsTest/Peach.Pro.MutatorsTest.csproj`：移除不存在的 `app.config` 复制项，使空测试壳可以构建。
- `pro/Test/Core/Peach.Pro.Test.csproj`：升级 NUnit、Moq、Newtonsoft.Json 和测试 SDK；重新启用迁移后的 REST Agent 测试，并加入 .NET 8 Agent、Worker、测试辅助程序引用及嵌入 PIT 资源。
- `pro/Test/Core/Storage/JobTests.cs`、`pro/Test/Core/Resources/pit.json`：修正现代 SQLite 对字符串字面量的解析，并补齐配置注入测试需要的最小 PIT 资源。
- `pro/Test/OS/Linux/Peach.Pro.Test.OS.Linux.csproj`：改为现代测试项目并引用迁移后的 Core/Pro 项目。
- `pro/Validator/PeachValidator.csproj`、`pro/XmlGenerator/PeachXmlGenerator.csproj`：转换为 `net8.0-windows` WinForms 项目，启用 `EnableWindowsTargeting`，并改用项目引用。

## 未迁移的功能和原因

以下功能没有通过简单改目标框架强行纳入，因为它们依赖已经从 .NET 8 移除或不再支持的运行时模型，需要单独重写。

1. **经典 Web UI / WebApi2**

   `pro/WebApi2` 使用 `System.Web`、OWIN 和 Nancy 等经典 ASP.NET 组件。它不是普通类库升级，必须重新设计为 ASP.NET Core/Minimal API 或其他现代宿主。因此当前 `Peach` 和 `PeachService` 以无嵌入 Web UI 模式运行。

2. **旧 .NET Remoting 兼容协议**

   `AgentTcpRemoting` 和 Unix trampoline 的 IPC 模式依赖 .NET Remoting，而 .NET 8 没有该运行时。默认 `tcp://` Agent 已切换为迁移后的 HTTP/WebSocket 显式协议；只剩用于旧节点互通的 `legacy` remoting 协议不可用。执行型 Unix trampoline 仍可构建。

3. **仓库中缺失的商业插件与 PIT 资产**

   测试和文档引用了源码树中不存在的 `TcpPort`、`Memory`、`Rest`、ASN.1、JSON data element、DNP3 PIT、若干 crypt transformer 以及 Titanium Web Proxy。这些无法仅靠改目标框架恢复，需要取得原插件源码/资源或重新实现。

4. **COM publisher/container**

   COM 组件只在 Windows 上有意义，并依赖注册表、ProgID/CLSID 和 Win32 容器。当前保留可移植 Pro 构建所需的其他 Windows 代码，但不把 COM publisher/container 宣称为已迁移功能；需要 Windows 专项验证和隔离项目。

5. **部分硬件和原生依赖 publisher**

   CAN、pcap/raw packet、串口、Bluetooth、SSL 等代码尽量纳入了 Pro 核心编译，但其完整集成测试仍需要设备、驱动、`libpcap`/`wpcap`、特权 capability 或特定系统库。WebSocket 已用真实客户端完成协议往返验证。

6. **旧数据库和依赖包兼容层**

   Job database 及部分 Dapper、DotNetZip、Nustache、PacketDotNet、SharpPcap、WebSocket 等包仍使用 .NET Framework 兼容资产。这样可以先完成源码迁移，但不等同于依赖已经现代化；替换这些包需要独立的行为和数据兼容工作。

7. **Windows GUI 的本机验证**

   Validator 和 XmlGenerator 已转换为 `net8.0-windows`，但当前执行环境没有 WindowsDesktop SDK，因此这里只能验证项目配置和跨项目引用，不能在 Linux 环境证明 WinForms 二进制可运行。

## 验证结果

- `dotnet build Peach.Build.csproj`：成功，0 errors（当前 Debug 构建 96 warnings）。
- Core 测试：138 passed，1 skipped。
- ProcessMonitor + RunCommand：38/38 passed。
- JobLogger：11/11 passed。
- WebSocket publisher：2/2 passed；Agent REST：13/15 passed，另 2 项只因引用的商业插件源码不在仓库。
- 命令行 Agent + 真实远程 UDP publisher：通过。
- Peach CLI 对 `pro/Peach/samples/HelloWorld.xml` 校验成功，单次迭代成功输出 `Hello World!`。

Linux monitor、pcap、GDB、旧 agent、Web UI 和特权相关测试仍应在具备对应系统依赖的专用环境中继续验证。
