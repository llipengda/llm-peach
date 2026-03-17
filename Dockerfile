FROM debian:stretch-slim AS base

RUN sed -i 's|deb.debian.org|archive.debian.org|g' /etc/apt/sources.list && \
    sed -i 's|security.debian.org|archive.debian.org|g' /etc/apt/sources.list && \
    sed -i '/stretch-updates/d' /etc/apt/sources.list && \
    apt-get update

RUN apt-get update && apt-get install -y \
    libpcap0.8 \
    python2.7 && \
    ln -s /usr/bin/python2.7 /usr/bin/python

#############################################################################################################################
FROM base AS builder

RUN apt-get install -y \
    build-essential \
    g++-multilib \
    doxygen \
    gnupg \
    dirmngr \
    ca-certificates \
    apt-transport-https \
    libpng-dev \
    wget vim git gdb \
    libpcap0.8-dev \
    nodejs node-typescript

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb https://download.mono-project.com/repo/debian stable-stretch main" | tee /etc/apt/sources.list.d/mono-official-stable.list && \
    apt-get update -y && \
    apt-get install -y mono-devel && \
    mozroots --import --sync

WORKDIR /peach/3rdParty/pin

RUN wget https://software.intel.com/sites/landingpage/pintool/downloads/pin-3.19-98425-gd666b2bee-gcc-linux.tar.gz && \
    tar -xzf pin-3.19-98425-gd666b2bee-gcc-linux.tar.gz && \
    mv pin-3.19-98425-gd666b2bee-gcc-linux pin-3.19-98425-gcc-linux && \
    rm pin-3.19-98425-gd666b2bee-gcc-linux.tar.gz

WORKDIR /peach

COPY . .

RUN sed -i '1i #define STATIC_ASSERT(cond)' core/BasicBlocks/bblocks.cpp

RUN ./waf configure

RUN ./waf build --variant=linux_x86_64_release

#############################################################################################################################
FROM base AS runner

RUN apt-get install -y \
    mono-complete && \    
    rm -rf /var/lib/apt/lists/*

COPY --from=builder /peach /peach

WORKDIR /peach

RUN ./waf install --variant=linux_x86_64_release

ENV PROTOCOL=mqtt \
    STRATEGY=random \
    MODE=llm-peach \
    LOG_DIR=/peach/logs \
    HOST=host.docker.internal \
    PORT=1883 \
    TIMEOUT=200 \
    FIXUP=1 \
    COUNT_PKT=0 \
    MUTATION_PER_ELEM=3 \
    PEACH_ARGS=

CMD ["./entrypoint.sh"]