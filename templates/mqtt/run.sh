#!/bin/bash

capitalize() {
    echo "$1" | sed 's/.*/\L&/; s/^\(.\)/\U\1/'
}

MUTATORS=""
LOGGER=""
FIXUP_XML=""
STRATEGY=$(capitalize "$STRATEGY")
MODE=$(capitalize "$MODE")
FIXUP=$FIXUP
LOG_DIR=$LOG_DIR
HOST=$HOST
PORT=$PORT
TIMEOUT=$TIMEOUT
PEACH_ARGS=$PEACH_ARGS


if [ "$MODE" == "peach" ]; then
    MUTATORS='<Mutators mode="exclude">' ++
                $(cat ./llm_mutators.txt) ++
             '</Mutators>'
elif [ "$MODE" == "llm" ]; then
    MUTATORS='<Mutators mode="include">' ++
                $(cat ./llm_mutators.txt) ++
             '</Mutators>'
elif [ "$MODE" == "llm-peach" ]; then
    MUTATORS=""
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

sed -e "s/@STRATEGY@/$STRATEGY/g" \
    -e "s|@MUTATORS@|$MUTATORS|g" \
    -e "s|@LOGGER@|$LOGGER|g" \
    -e "s|@FIXUP@|$FIXUP_XML|g" \
    -e "s/@HOST@/$HOST/g" \
    -e "s/@PORT@/$PORT/g" \
    -e "s/@TIMEOUT@/$TIMEOUT/g" \
    ./mqtt.xml.template > ./mqtt.xml


/peach/output/linux_x86_64_release/bin/peach \
    ./mqtt.xml $PEACH_ARGS