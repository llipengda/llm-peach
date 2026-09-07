FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN bash build/install.sh /out/Peach

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /opt/peach
COPY --from=build /out/Peach/ ./

WORKDIR /work
ENTRYPOINT ["dotnet", "/opt/peach/Peach.dll"]
CMD ["--help"]
