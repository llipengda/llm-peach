#!/bin/bash

capitalize() {
    echo "$1" | sed 's/^\(.\)/\U\1/'
}

MUTATORS=""
LOGGER=""
FIXUP_XML=""
IMPORTS=""
ON_COMPLETE=""
STRATEGY_XML=""
STRATEGY=$(capitalize "$STRATEGY")
MODE=$MODE
FIXUP=$FIXUP
LOG_DIR=$LOG_DIR
HOST=$HOST
PORT=$PORT
TIMEOUT=$TIMEOUT
PEACH_ARGS=$PEACH_ARGS
COUNT_PKT=$COUNT_PKT


if [ "$MODE" == "peach" ]; then
    MUTATORS='<Mutators mode="exclude">'$(tr -d '\n' < ./llm_mutators.txt)'</Mutators>'
elif [ "$MODE" == "llm" ]; then
    MUTATORS='<Mutators mode="include">'$(tr -d '\n' < ./llm_mutators.txt)'</Mutators>'
elif [ "$MODE" == "llm-peach" ]; then
    MUTATORS=""
else
    echo "Unknown MODE: $MODE, must be one of: peach, llm, llm-peach"
    exit 1
fi

if [ -n "$LOG_DIR" ]; then
    LOGGER='<Logger class="File"><Param name="Path" value="'"$LOG_DIR"'"/></Logger>'
else
    LOGGER=""
fi

if [ "$FIXUP" -eq 1 ]; then
    FIXUP_XML='<Fixup class="DtlsFixup"><Param name="ref" value="dtls_record"/></Fixup>'
else
    FIXUP_XML=""
fi

if [ "$COUNT_PKT" -eq 1 ]; then
    IMPORTS='<Import import="pkt_cnt"/>'
    ON_COMPLETE='onComplete="pkt_cnt.count_pkt(self)"'
fi

if [ "$STRATEGY" == "TwoPhaseRandom" ]; then
    STRATEGY_XML='<Strategy class="TwoPhaseRandom"><Param name="TwoPhaseMutation" value="True" /><Param name="MultipleMutationsPerElement" value="3" /></Strategy>'
else
    STRATEGY_XML="<Strategy class=\"$STRATEGY\" />"
fi

pit_file="./dtls_STRATEGY=${STRATEGY}&MODE=${MODE}&FIXUP=${FIXUP}&HOST=${HOST}&PORT=${PORT}&PEACH_ARGS=${PEACH_ARGS// /_}.xml"

sed -e "s|@STRATEGY@|$STRATEGY_XML|g" \
    -e "s|@MUTATORS@|$MUTATORS|g" \
    -e "s|@LOGGER@|$LOGGER|g" \
    -e "s|@FIXUP@|$FIXUP_XML|g" \
    -e "s/@HOST@/$HOST/g" \
    -e "s/@PORT@/$PORT/g" \
    -e "s/@TIMEOUT@/$TIMEOUT/g" \
    -e "s|@IMPORTS@|$IMPORTS|g" \
    -e "s/@ON_COMPLETE@/$ON_COMPLETE/g" \
    ./dtls.xml.template > "$pit_file"

[[ $PEACH_ARGS == \"*\" ]] && PEACH_ARGS=${PEACH_ARGS#\"} && PEACH_ARGS=${PEACH_ARGS%\"}

exec /peach/output/linux_x86_64_release/bin/peach \
    "$pit_file" $PEACH_ARGS