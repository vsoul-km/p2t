
# p2t
Advanced ping command line utility for Windows (uses .NET Framework 4.6)

.NET 4.6 command line utility to ping hosts by IP/HostName (like standard Windows ping) and runs traceroute if ping fails.
Could send an output to Telegram.
Runs in interactive mode if started without arguments.

**Usage:**

    Usage: p2t.exe ipaddress/name -option1 -option2 -option...
    options:
        -l size   Packet payload size, optional. Default is 32 byte.
        -c count  Number of ping echo requests, optional. Default is infinite.
        -w ms     Timeout in ms. to wait for each reply, optional. Default is 2000 ms.
        -i ms     Interval between RTT in ms, optional. Default is 500 ms.
        -f        Do not fragment. The default is false (disabled).
        -d        Add date to each ping output. The default is false (disabled).
        -e        Errors only. Displays and logs only ping errors. The default is false (disabled).
        -notrace  Disable trace route on ping error. The default is false (disabled).
        -log      Write output to log file.
        -follow   Follow the hostname. Do not fix IP address when resolving a name for the first time.
                  The name will resolve to the IP address at every ping. The default is false (disabled).
                  Only works when using the host name as the address.
        -t        Send output using a Telegram bot. The default is false (disabled).
                  To create a bot follow the instruction: https://core.telegram.org/bots#creating-a-new-bot.
                  If defined, arguments -tt and -tc should be also defined.
        -tt       Telegram bot access token. If defined, you can omit the -t argument.
                  If defined, argument -tc should be also defined.
        -tc       Telegram bot Chat ID.
                  To detect Chat ID follow the instruction: https://github.com/vsoul-km/p2t/wiki/Detect-Telegram-Chat-ID.
        -ta       Duplicate all output to Telegram. The default is false (disabled).
        -te       Only send errors to Telegram. The default is true (enabled).
    
    Examples:
              p2t.exe 8.8.8.8
              p2t.exe www.google.com -log
              p2t.exe 8.8.8.8 -l 1400 -c 100 -i 250 -log
              p2t.exe www.google.com -l 1400 -c 100 -i 2000 -log -follow -d -tt 1032560943:AAG3-S4-v4fOwRpyIr1KggKnQZDobFyCa-A -tc 315147040 -ta

## Output examples

**Launched with arguments:**

Command:

    >p2t.exe www.google.com -l 1400 -c 5 -i 2000 -log -follow -d -tt 1183030956:AAG3-S4-v4NowRpyIr1KNgKnQZsobGyCa-A -tc 374137042 -te

Console output:

    Using the following file to log: C:\Temp\p2t_16-01-2022_20-59-11_www.google.com.log
    
    p2t.exe v1.2.0.0
    Ping started. Used options:
     Address: www.google.com
     Is address: False
     Packet Size: 1400 bytes
     Ping Count: 5
     Timeout: 2000 ms
     Interval: 2000 ms
     Don't Fragment: False
     Follow the Name: True
     Add date to each ping output: False
     Errors only: False
     No trace: False
     Using Telegram bot to send errors: True
     Telegram bot channel id: 374137042
     Telegram, only send errors: True
    
    [20:59:11.737]  Reply from: 142.250.185.68  fragment=True  bytes=1400  time=44ms  TTL=54
    [20:59:13.784]  Reply from: 142.250.185.68  fragment=True  bytes=1400  time=42ms  TTL=54
    [20:59:15.836]  Reply from: 142.250.185.68  fragment=True  bytes=1400  time=42ms  TTL=54
    [20:59:17.883]  Reply from: 142.250.185.68  fragment=True  bytes=1400  time=42ms  TTL=54
    [20:59:19.930]  Reply from: 142.250.185.68  fragment=True  bytes=1400  time=42ms  TTL=54
    
    Packets: sent - 5; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 42 ms; Unique IP addresses: 1
    Used IP addresses: 142.250.185.68

**Launched Interactively:**

Command:

    >p2t.exe

Console output:

    Application is running without any arguments. Starting an interactive mode.
    Please enter parameters following this step-by-step procedure:
    
    Enter the IP address or the hostname to ping: www.google.com
    Enter the interval between RTT in ms (default is 500 ms):
    Enable trace route on ping fails? (y/n)?: y
    Follow the name? (y/n)?: y
    Do you want to log output to file (y/n)?: y
    Using the following file to log: C:\Temp\p2t_16-01-2022_21-02-00_www.google.com.log
    
    p2t.exe v1.2.0.0
    Ping started. Used options:
     Address: www.google.com
     Is address: False
     Packet Size: 32 bytes
     Ping Count: infinite
     Timeout: 2000 ms
     Interval: 500 ms
     Don't Fragment: False
     Follow the Name: True
     Add date to each ping output: False
     Errors only: False
     No trace: False
     Using Telegram bot to send errors: False
    
    [21:02:36.100]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:36.647]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:37.194]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:37.741]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:38.287]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:38.834]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:39.384]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:39.931]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:40.478]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:41.024]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=43ms  TTL=54
    [21:02:41.571]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    [21:02:42.118]  Reply from: 142.250.185.68  fragment=True  bytes=32  time=42ms  TTL=54
    
    Packets: sent - 12; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 42 ms; Unique IP addresses: 1
    Used IP addresses: 142.250.185.68
    
    Press any key to exit...

## **v1.2.0.0**

Added -notrace argument to disable trace routes if ping error
Added -e argument to display or log ping errors only
Improved Follow the hostname mode
Fixed bug with the trace route interruption
Removed "Press any key" on exit from the p2t.exe if was launched in interactive mode


## **v1.1.0.0**

Added possibility to send output to Telegram.
Switched to .NET Framework 4.6

## **v1.0.0.1**

Added -d argument to add date to each ping output

## **v1.0.0.0**

First release

## Other notes

VS 2019 solution.
