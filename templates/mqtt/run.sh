#!/bin/bash

capitalize() {
    echo "$1" | sed 's/.*/\L&/; s/^\(.\)/\U\1/'
}

MUTATORS=""
LOGGER=""
FIXUP_XML=""
STRATEGY=$(capitalize "$STRATEGY")
MODE=$MODE
FIXUP=$FIXUP
LOG_DIR=$LOG_DIR
HOST=$HOST
PORT=$PORT
TIMEOUT=$TIMEOUT
PEACH_ARGS=$PEACH_ARGS


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
    FIXUP_XML='<Fixup class="MqttFixup"><Param name="ref" value="packets"/></Fixup>'
else
    FIXUP_XML=""
fi

pit_file="./mqtt_STRATEGY=${STRATEGY}&MODE=${MODE}&FIXUP=${FIXUP}&HOST=${HOST}&PORT=${PORT}&PEACH_ARGS=${PEACH_ARGS}.xml"

sed -e "s/@STRATEGY@/$STRATEGY/g" \
    -e "s|@MUTATORS@|$MUTATORS|g" \
    -e "s|@LOGGER@|$LOGGER|g" \
    -e "s|@FIXUP@|$FIXUP_XML|g" \
    -e "s/@HOST@/$HOST/g" \
    -e "s/@PORT@/$PORT/g" \
    -e "s/@TIMEOUT@/$TIMEOUT/g" \
    ./mqtt.xml.template > $pit_file


/peach/output/linux_x86_64_release/bin/peach \
    $pit_file $PEACH_ARGS