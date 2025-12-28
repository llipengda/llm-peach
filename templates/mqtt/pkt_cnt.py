total_pkt = 0
iteration = 0
first = True
f = None

import os
import datetime

def init(output):
    global f
    time = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    file_dir = "/peach/logs/pkt_count/%s_%s" % (output.parent.parent.parent.context.config.pitFile.rsplit('/', 1)[1], time)
    if not os.path.exists(file_dir):
        os.makedirs(file_dir)
    csv_file = "%s/packets.csv" % file_dir
    f = open(csv_file, 'w')
    f.write("time,packets\n")
    
def write_to_file():
    global f
    global total_pkt
    global iteration

    time = datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    f.write("%s,%d\n" % (time, total_pkt))
    f.flush()
    
def finish(output):
    write_to_file()
    f.close()
    print("Finished writing packet counts.")

def count_pkt(output):
    global total_pkt
    global first
    global iteration
    iteration += 1
    total_pkt += int(output.data.dataModel.find('packets').Count)
    if first:
        first = False
        init(output)
        output.parent.parent.parent.context.engine.TestFinished += finish
    if iteration % 1000 == 0:
        write_to_file()