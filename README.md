<img src="assets/NICE Logo.png" alt="logo" width="200" align="left"/>

## Network Interface Communication Emulator

[![Build Status](https://travis-ci.org/Azer0s/NICE.svg?branch=master)](https://travis-ci.org/Azer0s/NICE)
[![License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/Azer0s/NICE/blob/master/LICENSE)

The Network Interface Communication Emulator (or NICE) is a C# framework which allows you to emulate entire networks in code. NICE implements Ethernet and several layer 3 protocols. Furthermore, it offers several Ethernet devices like switches or routers.

### NICE is still (very much) WIP, help is appreciated 

- [ ] Layer 2
  - [x] Ethernet
  - [x] VLAN
  - [x] Switching
  - [x] Async Ethernet frames
  - [ ] STP
- [ ] Layer 3
  - [ ] IPv4
  - [ ] ARP
  - [ ] ICMP
  - [ ] Routing
  - [ ] (Eventually: routing protocols)
- [ ] Layer 4
  - [ ] TCP
  - [ ] UDP
- [ ] Layer 7
  - [ ] DNS
  - [ ] HTTP
  - [ ] FTP
  - [ ] Telnet

## Samples

### Sample network (2 end devices, 2 switches)

```cs
using System;
using NICE.Foundation;
using NICE.Hardware;
using NICE.Layer2;
using NICE.Layer3;
using NICE.Layer4;

using static NICE.API.Generators;

Log.SetLevel(Log.Level.TRACE, Log.Groups.SHOW);
Vlan.Register(1, "DEFAULT");

/*
 * PC1 ---  eth0/1
 *       |
 *       | fa0/1
 *      SW1
 *       |  fa0/2
 *       |
 *       | fa0/1
 *      SW2
 *       |  fa0/1
 *       |
 * PC2 --- eth0/1
 */

var pc1 = new EthernetDevice("pc1");
var pc2 = new EthernetDevice("pc2");

var sw1 = new EthernetSwitch("sw1");
var sw2 = new EthernetSwitch("sw2");

//pc ports
pc1[ETH01].Init();
pc2[ETH01].Init();

//Connection from sw1 to pc1
sw1[FA01].Init();

//Connection from sw2 to pc2
sw2[FA01].Init();

//Connection from sw1 to sw2
sw1[FA02].Init();
sw2[FA02].Init();

//Connect the pcs to the switches
pc1[ETH01].ConnectTo(sw1[FA01]);
pc2[ETH01].ConnectTo(sw2[FA01]);

//Connect the switches to each other
sw1[FA02].ConnectTo(sw2[FA02]);

//Set the ports from pc to switch to access vlan 1
sw1.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));
sw2.SetPort(FA01, EthernetSwitch.AccessMode.ACCESS, Vlan.Get(1));

//Set the ports from switch to switch to trunk
sw1.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);
sw2.SetPort(FA02, EthernetSwitch.AccessMode.TRUNK, null);


//Learn MAC Addresses
Log.Group("Learn MAC Addresses");

/*
 * The API can be used with constructors (like this)
 */
pc1[ETH01].SendSync(new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc1[ETH01], Vlan.Get(1), new RawPacket(new byte[100])));

//Wait for all sending operations to be finished (you don't HAVE to wait...I just prefer doing so, cause the log is more readable)
//This is necessary cause even tho you send this frame synchronously, all the connected devices create new tasks for incoming frames 
await Global.WaitForOperationsFinished();

/*
 * Or like this (with static methods and a scapy-esque construction pipeline)
 */
pc2[ETH01].SendSync(Ethernet(Constants.ETHERNET_BROADCAST_ADDRESS, pc2[ETH01]) | Dot1Q(Vlan.Get(1)) | RawPacket(new byte[100]));
await Global.WaitForOperationsFinished();

Log.Group("Send Ethernet frame over learned ports");
pc1[ETH01].SendAsync(Ethernet(pc2[ETH01], pc1[ETH01]) | Dot1Q(Vlan.Get(1)) | RawPacket(new byte[100]));
await Global.WaitForOperationsFinished();
```
