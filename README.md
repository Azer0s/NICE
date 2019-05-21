<img src="assets/NICE Logo.png" alt="logo" width="200" align="left"/>

## Network Interface Communication Emulator

[![Build Status](https://travis-ci.org/Azer0s/NICE.svg?branch=master)](https://travis-ci.org/Azer0s/NICE)
[![License](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/Azer0s/NICE/blob/master/LICENSE)

The Network Interface Communication Emulator (or NICE) is a C# framework which allows you to emulate entire networks in code. NICE implements Ethernet and several layer 3 protocols. Furthermore, it offers several Ethernet devices like switches or routers.

### NICE is still (very much) WIP, help is appreciated 

- [x] Layer 2
  - [x] Ethernet
  - [x] VLAN
  - [x] Switching
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
using NICE;
using NICE.Hardware;
using NICE.Layer2;
using NICE.Layer3;

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

pc1[ETH01].Send(new EthernetFrame(Constants.ETHERNET_BROADCAST_PORT, pc1[ETH01], null, EtherType.IPv4, new RawLayer3Packet(new byte[100]), false));

```
