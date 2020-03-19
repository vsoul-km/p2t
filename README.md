
# p2t
Advanced ping command line utility for Windows (uses .NET Framework 2.0)

.NET 2.0 command line utility to ping hosts by IP/HostName (like standard Windows ping) and runs traceroute if ping fails.
Runs in interactive mode if started without arguments.

**Usage:**

    Usage: p2t.exe ipaddress/name -option1 -option2 -option...
    options:
        -l size   Packet payload size, optional. Default is 32 byte.
        -c count  Number of ping echo requests, optional. Default is infinite.
        -w ms     Timeout in ms. to wait for each reply, optional. Default is 2000 ms.
        -i ms     Interval between pings RTT in ms, optional. Default is 500 ms.
        -f        Do not fragment. The default is false (disabled).
        -log      Write output to log file.
        -follow   Follow the hostname. Do not fix IP address when resolving a name for the first time.
                  The name will resolve to the IP address at every ping. The default is false (disabled).
                  Only works when using the host name as the address.
    
    Examples:
              p2t.exe 8.8.8.8
              p2t.exe www.google.com -log
              p2t.exe 8.8.8.8 -l 1500 -c 100 -i 250 -log
              p2t.exe www.google.com -l 1500 -c 100 -i 250 -log -follow

## Output examples

**Launched with arguments:**

Command:

    >d:\p2t.exe www.google.com -l 128 -c 10 -w 2000 -i 100 -f -log -follow

Console output:

    Using the following file to log: D:\p2t_19-03-2020_16-35-56_www.google.com.log
        
    Host Name: www.google.com
    IP Address: www.google.com
    Packet Size: 128 bytes
    Ping Count: 10
    Timeout: 2000 ms
    Interval: 100 ms
    Don't fragment: True
    Follow the Name: True
    
    16:35:57.246  Reply from: 172.217.21.132  fragment=False  bytes=128  time=49ms  TTL=48
    16:35:57.418  Reply from: 172.217.21.132  fragment=False  bytes=128  time=49ms  TTL=48
    16:35:57.574  Reply from: 172.217.21.132  fragment=False  bytes=128  time=54ms  TTL=48
    16:35:57.746  Reply from: 172.217.21.132  fragment=False  bytes=128  time=48ms  TTL=48
    16:35:57.902  Reply from: 172.217.21.132  fragment=False  bytes=128  time=48ms  TTL=48
    16:35:58.056  Reply from: 172.217.21.132  fragment=False  bytes=128  time=49ms  TTL=48
    16:35:58.233  Reply from: 172.217.21.132  fragment=False  bytes=128  time=49ms  TTL=48
    16:35:58.396  Reply from: 172.217.21.132  fragment=False  bytes=128  time=48ms  TTL=48
    16:35:58.553  Reply from: 172.217.21.132  fragment=False  bytes=128  time=48ms  TTL=48
    16:35:58.711  Reply from: 172.217.21.132  fragment=False  bytes=128  time=50ms  TTL=48

    Packets: sent - 10; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 49 ms; Unique IP addresses: 1
    Used IP's:
      172.217.21.132
    
    Press any key to exit...

**Launched Interactively:**

Command:

    >d:\p2t.exe

Console output:

    Application is running without any arguments. Starting an interactive mode.
    Please enter parameters following this step-by-step procedure:
    
    Enter the IP address or the hostname to ping: www.google.com
    Enter the RTT interval in ms (default is 500 ms): 100
    Do you want to log ping to file (y/n)?: y
    Using the following file to log: D:\p2t_19-03-2020_16-42-54_172.217.21.164.log
    
    Host Name: www.google.com
    IP Address: 172.217.21.164
    Packet Size: 32 bytes
    Ping Count: infinite
    Timeout: 2000 ms
    Interval: 100 ms
    Don't fragment: False
    Follow the Name: False
    
    16:43:04.210  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    16:43:04.382  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    16:43:04.538  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    16:43:04.694  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    16:43:04.850  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    16:43:05.007  Reply from: 172.217.21.164  fragment=True  bytes=32  time=49ms  TTL=48
    
    Packets: sent - 6; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 49 ms; Unique IP addresses: 0
    
    Press any key to exit...

## **Version 1.0.0**

First release

## Other notes

VS 2019 project.
