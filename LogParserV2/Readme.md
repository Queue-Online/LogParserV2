# IIS Log Parser

This document explains how to use the IIS Log Parser to parse IIS logs from a folder, display the results, and identify potential security issues. The parser can be run in a Docker container, which makes it easy to manage and deploy.

## Running the Log Parser

### Basic Usage

To parse logs from a local directory, you can run the Docker container with the following command:

```bash
docker run --rm -v /c/temp:/app/logs logparser-v2
docker run --rm -v /path/to/logs:/app/logs -e LOG_PATH=/app/logs logparser-v2 "/app/logs" "2024-08-01" "2024-08-31"



Identified log format: W3C

Unique Users:
jane_smith      | ################################################## 50
john_doe        | ################################################## 50

Total Unique Users: 2

All Users:
Anonymous       | ################################################## 120
jane_smith      | #################### 50
john_doe        | #################### 50

Total User Entries: 3

IP Addresses:
192.168.1.121   | ########## 10
192.168.1.120   | ########## 10
192.168.1.119   | ########## 10
192.168.1.118   | ########## 10
192.168.1.117   | ########## 10
192.168.1.116   | ########## 10
192.168.1.115   | ########## 10
192.168.1.114   | ########## 10
192.168.1.113   | ########## 10
192.168.1.112   | ########## 10
192.168.1.111   | ########## 10
192.168.1.110   | ########## 10
192.168.1.109   | ########## 10
192.168.1.108   | ########## 10
192.168.1.107   | ########## 10
192.168.1.106   | ########## 10
192.168.1.105   | ########## 10
192.168.1.104   | ########## 10
192.168.1.103   | ########## 10
192.168.1.102   | ########## 10
192.168.1.101   | ########## 10
192.168.1.100   | ########## 10

Total Unique IP Addresses: 22

Time Taken (ms):
98
56
78
89
123
156
... (truncated for brevity)

Suspicious IP Addresses (more than 100 requests):

Unsuccessful Login Attempts:

Access to Sensitive Resources:
/login - 10 accesses

