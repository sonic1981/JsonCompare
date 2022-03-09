# JSON Compare

this repository exists to show off some of the work I've undertaken around this area. This project was designed to be part of a much bigger project. 

It is a part of a system that was designed to accept two JSON files and to copare these fiels to find deltas. The idea that these deltas would highlight differences in a computer systems configuration. 

It was built using a pure TDD approach where multiple example files were generated and run though the unit tests to find bugs and weaknesses in the software. 

## Difference Engine

The core of the system is the [`DiffEngine`](https://github.com/sonic1981/YamlCompare/blob/master/Compare/Snapshot.Compare.Core/Differences/DiffEngine.cs) class. This is the main part of the system that processes the input and produces the required output. As a pur logic microservice it lent itself very well to unit testing and TDD.

The process supports a basic JSON structre roughly equiverlent to:

```json
{
    "intent": []
    "intent": {}
}

```

Examples of the various file inputs can be [seen in the tests](https://github.com/sonic1981/JsonCompare/tree/master/Tests).