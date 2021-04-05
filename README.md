
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

    >p2t.exe www.google.com -l 1400 -c 100 -i 2000 -log -follow -d -tt 1183030956:AAG3-S4-v4NowRpyIr1KNgKnQZsobGyCa-A -tc 374137042 -te

Console output:

    Using the following file to log: C:\Temp\p2t_06-04-2021_00-50-04_www.google.com.log
    
    p2t.exe v1.1.0.0
    Ping started. Used options:
    Host Name: www.google.com
    IP Address: www.google.com
    Packet Size: 1400 bytes
    Ping Count: 100
    Timeout: 2000 ms
    Interval: 2000 ms
    Don't Fragment: False
    Follow the Name: True
    Add Date to each ping output: True
    Using Telegram bot to send errors: True
    Telegram bot channel id: 374137042
    Telegram, only send errors: True
    
    [06-04-2021 00:50:04.794]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=31ms  TTL=113
    [06-04-2021 00:50:06.827]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    [06-04-2021 00:50:08.861]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    [06-04-2021 00:50:10.894]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    [06-04-2021 00:50:12.927]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    [06-04-2021 00:50:14.960]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    [06-04-2021 00:50:16.993]  Reply from: 216.58.209.4  fragment=True  bytes=1400  time=30ms  TTL=113
    
    Packets: sent - 7; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 30 ms; Unique IP addresses: 1
    Used IP addresses: 216.58.209.4
    
    Press any key to exit...
	
Telegram output:	

    vSoul_bot, [6.04.2021 at 00:50:04]:
    p2t.exe v1.1.0.0
    Ping started. Used options:
    Host Name: www.google.com
    IP Address: www.google.com
    Packet Size: 1400 bytes
    Ping Count: 100
    Timeout: 2000 ms
    Interval: 2000 ms
    Don't Fragment: False
    Follow the Name: True
    Add Date to each ping output: True
    Using Telegram bot to send errors: True
    Telegram bot channel id: 374137042
    Telegram, only send errors: True
    
    Packets: sent - 7; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 30 ms; Unique IP addresses: 1
    
    Used IP addresses: 216.58.209.4

**Launched Interactively:**

Command:

    >p2t.exe

Console output:

    Application is running without any arguments. Starting an interactive mode.
    Please enter parameters following this step-by-step procedure:
    
    Enter the IP address or the hostname to ping: www.google.com
    Enter the interval between RTT in ms (default is 500 ms):
    Do you want to log ping to file (y/n)?: n
    
    p2t.exe v1.1.0.0
    Ping started. Used options:
    Host Name: www.google.com
    IP Address: 216.58.215.68
    Packet Size: 32 bytes
    Ping Count: infinite
    Timeout: 2000 ms
    Interval: 500 ms
    Don't Fragment: False
    Follow the Name: False
    Add Date to each ping output: False
    Using Telegram bot to send errors: False
    
    [00:56:02.105]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=32ms  TTL=113
    [00:56:02.639]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=31ms  TTL=113
    [00:56:03.172]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=30ms  TTL=113
    [00:56:03.704]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=31ms  TTL=113
    [00:56:04.236]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=31ms  TTL=113
    [00:56:04.769]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=30ms  TTL=113
    [00:56:05.301]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=30ms  TTL=113
    [00:56:05.833]  Reply from: 216.58.215.68  fragment=True  bytes=32  time=31ms  TTL=113
    
    Packets: sent - 8; Lost - 0 (0%); Traceroutes: 0; Avg.RTT: 30 ms; Unique IP addresses: 0
    
    Press any key to exit...

## **v1.1.0.0**

Added possibility to send output to Telegram.
Switched to .NET Framework 4.6

## **v1.0.0.1**

Added -d switch to add date to each ping output

## **v1.0.0.0**

First release

## Other notes

VS 2019 solution.
