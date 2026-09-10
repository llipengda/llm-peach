FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN apt-get update \
	&& apt-get install -y --no-install-recommends build-essential \
	&& rm -rf /var/lib/apt/lists/*
WORKDIR /src
COPY . .
RUN bash build/install.sh /out/Peach

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /opt/peach
COPY --from=build /out/Peach/ ./

WORKDIR /work
ENTRYPOINT ["dotnet", "/opt/peach/Peach.dll"]
CMD ["--help"]
