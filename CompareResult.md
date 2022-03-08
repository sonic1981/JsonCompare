# Using the result of the snapshot compare function

## Overview

This document is designed to give the consumer an understanding of the data structure of the result of the snapshot compare function. The compare function takes two snapshot id’s and performs a diff procedure on the files to produce the delta of these machine snapshots. 

## Overall structure

The result of is an array of objects in JSON format with the structure:

```json
[
    {
        "intent": "adusergroups",
        "arrayDifferences1": [],
        "arrayDifferences2": [],
        "objectDifferences": [],
        "inError1": true,
        "inError2": true
    }, {}, {}…
]
```

### intent

So each result is an “intent”. An intent in Edge Intelligence (EI) is the result of one sensor. So the intent “adusergroups” is the result of the “active directory user groups” sensor. More information on this can be found here https://realtimedocumentation.azurewebsites.net/

### Results

Each intent result contains the differences snapshot found in that intents reponses. Each result is numbered 1 and 2, with 1 being the first result requested and 2 being the second result requested in the request to the function, e.g.

```json
{"Result1":"cc5d46f7-186a-4b22-8f90-52a73f6733ad",
"Result2":"6c5e8df1-dad5-4d46-961b-7b304ca2d54a"}
```

### Errors

If either of the results contained errors for whatever reason, EI intents can error at certain times, the inError(x) response will be set to true. So if in this example, we only have errors returned for this intent:

```json
{
    "intent": "adusergroups",
    "arrayDifferences1": [],
    "arrayDifferences2": [],
    "objectDifferences": [],
    "inError1": true,
    "inError2": true
}
```

### Arrays vs objects

Edge intelligence returns its data in two forms, arrays of objects or objects. 

An object result is where only a single item is queried on the end point, for example the CPU sensor only returns information on what the CPU(s) are doing at that time so it returns an object result (note it does not return what each individual CPU is doing hence it is not an array.

An array result is returned when the data is more of a list of items, for example the active ports sensor returns an array of results, one for each port.

#### Object deltas

When an intent is an object intent the `objectDifferences` properly of the JSON will be populated and the `arrayDifferences1` and `arrayDifferences2` property will be empty.

In this scenario the compare function will look for differences in **each individual property** of the objects returned from the snapshot results. An example object delta will look like:

```json
{
  "intent": "cpu",
  "arrayDifferences1": [],
  "arrayDifferences2": [],
  "objectDifferences": [
      {
          "key": "processid",
          "value1": "3480",
          "value2": "740"
      },
      {
          "key": "processname",
          "value1": "sqlservr.exe",
          "value2": "lsass.exe"
      },
      {
          "key": "processcpu",
          "value1": "15",
          "value2": "9"
      },
      {
          "key": "cpuUsage",
          "value1": "64",
          "value2": "40"
      },
      {
          "key": "startTime",
          "value1": "2020-10-29T15:39:00.6819581Z",
          "value2": "2020-10-29T15:38:56.4298999Z"
      },
      {
          "key": "processPath",
          "value1": "sqlservr.exe",
          "value2": "lsass.exe"
      }
  ],
  "inError1": false,
  "inError2": false
}
```

The `objectDifferences` collection contains an object for each property of the intent returned with a value for the first and second result. **Only properties that vary between snapshots are returned**. So in the example above, if `processPath` contained the same value in both snapshots then this would be omitted from this result entirely.

An example of how this data could be displayed is:

|Sensor|<date>|<date>|
|------|------|------|
|cpu|processid: 3480| processid: 740|
|   |processname: sqlservr.exe | pocessname: lsass.exe|
|   | processcpu: 64 | processcpu: 40|


#### Array deltas

When an intent is an **array** intent the `arrayDifferences1` and `arrayDifferences2` properties of the JSON will be populated and the `objectDifferences` property will be empty.

For array based results we take a different approach, array results will always include items that are in the first result and not the second and/or items that are in the second result but not the first. If any value in the array properties differ then it is counted as a delta. For example; given two results that contain netstat values:

##### Result 1

```yaml
activeports:
- protocol: TCP
  localAddress: 172.16.162.130
  localPort: 13672
  remoteAddress: 23.63.74.191
  remoteHostName: ''
  remotePort: 80
  remotePortIanaServiceName: http
  state: TIME_WAIT
- protocol: TCP
  localAddress: 172.16.162.454
  localPort: 12564
  remoteAddress: 26.63.74.191
  remoteHostName: ''
  remotePort: 80
  remotePortIanaServiceName: http
  state: TIME_WAIT
```

##### Result 2

```yaml
activeports:
- protocol: TCP
  localAddress: 172.16.162.130
  localPort: 13672
  remoteAddress: 23.63.74.191
  remoteHostName: ''
  remotePort: 81
  remotePortIanaServiceName: http
  state: TIME_WAIT
- protocol: TCP
  localAddress: 172.16.162.454
  localPort: 12564
  remoteAddress: 26.63.74.191
  remoteHostName: ''
  remotePort: 80
  remotePortIanaServiceName: http
  state: TIME_WAIT
```

- As the second result in each set is identical this is not returned
- The first record in each differs on `remotePort` so this triggers it as a delta
- What is returned is both results one for value one and one for value 2:

```json
{
  "intent": "netstat",
  "arrayDifferences1": [
      {
          "values": [
              {
                  "key": "protocol",
                  "value": "TCP"
              },
              {
                  "key": "localAddress",
                  "value": "172.16.162.130"
              },
              {
                  "key": "localPort",
                  "value": "13672"
              },
              {
                  "key": "remoteAddress",
                  "value": "23.63.74.191"
              },
              {
                  "key": "remoteHostName",
                  "value": ""
              },
              {
                  "key": "remotePort",
                  "value": "80"
              },
              {
                  "key": "remotePortIanaServiceName",
                  "value": "http"
              },
              {
                  "key": "state",
                  "value": "TIME_WAIT"
              }
          ]
      },
      {
          "values": [
              {
                  "key": "protocol",
                  "value": "TCP"
              },
              {
                  "key": "localAddress",
                  "value": "172.16.162.130"
              },
              {
                  "key": "localPort",
                  "value": "13672"
              },
              {
                  "key": "remoteAddress",
                  "value": "23.63.74.191"
              },
              {
                  "key": "remoteHostName",
                  "value": ""
              },
              {
                  "key": "remotePort",
                  "value": "81"
              },
              {
                  "key": "remotePortIanaServiceName",
                  "value": "http"
              },
              {
                  "key": "state",
                  "value": "TIME_WAIT"
              }
          ]
      }
  ],
  "arrayDifferences2": [],
  "objectDifferences": [],
  "inError1": false,
  "inError2": false
}
```

This could then be displayed thus in the result:

|Sensor|<date>|<date>|
|------|------|------|
|netstat|protocol: TCP| protocol: TCP|
|   |localAddress: 172.16.162.130 | localAddress: 172.16.162.130|
|   | localPort: 13672 | localPort: 13672|
|   | remoteAddress: 23.63.74.191 | remoteAddress: 23.63.74.191|
|   | remoteHostName:  | remoteHostName: |
|   | remotePort: 80 | remoteHostName: 81 |
|   | remotePortIanaServiceName: http | remotePortIanaServiceName: http |
|   | state: TIME_WAIT | state: TIME_WAIT|

similary this then allows for items that are in one result but not another:

|Sensor|<date>|<date>|
|------|------|------|
|netstat|protocol: TCP| protocol: TCP|
|   |localAddress: 172.16.162.130 | localAddress: 172.16.162.130|
|   | localPort: 13672 | localPort: 13672|
|   | remoteAddress: 23.63.74.191 | remoteAddress: 23.63.74.191|
|   | remoteHostName:  | remoteHostName: |
|   | remotePort: 80 | remoteHostName: 81 |
|   | remotePortIanaServiceName: http | remotePortIanaServiceName: http |
|   | state: TIME_WAIT | state: TIME_WAIT|
|------|------|------|
|netstat|protocol: TCP| protocol: TCP|
|   |localAddress: 172.16.162.130 | |
|   | localPort: 13672 | |
|   | remoteAddress: 23.63.74.191 | |
|   | remoteHostName:  |  |
|   | remotePort: 80 |  |
|   | remotePortIanaServiceName: http |  |
|   | state: TIME_WAIT | |

etc.
