# Merkle File Server

Implemented solution is working as a server for downloading files using merkle tree algorithm.

### Start the server and load a file

File can be preloaded with startup argument: (_It just requires to <u>escape backslashes</u>_)
```
MerkleFileServer.Api.exe --file :fileFullName
```
works also without parameter name:
```
MerkleFileServer.Api.exe :fileFullName
```

or via dedicated api after the process is started: 
```
POST /hashes/:fileFullName
```

#### Specify port number

In order to set specific port number on which the api shall be hosted the url shall be passed while starting the process

```
MerkleFileServer.Api.exe --file :fileFullName --urls "https://localhost:7000"
```

#### Swagger

Api exposes swagger on:

https://localhost:7000/swagger/index.html

_respecting port number which instance started with_


### Download
After started and file is preloaded then there are two apis which could be used to download the file

```
GET /hashes
```
listing all available merkle hashes togather with number of pieces it consists of.

```
GET /piece/:hashId/:pieceIndex
```

allowing to fetch the file piece by piece. This comes togather with list of proofs that can be used to validate legitness of the piece

### Client addon

There also is a client functionality addon, meaning that it can also serve as consumer downloading a file.

It can download file pieces from many peers at the same time, validating proofs for each and merging it all togahter when full list of pieces is downloaded.

```
GET /asclient/:hashId/:destinationFile 
```
Above endpoint downloads piece with given hashId. 


Parameter   | Description
----------- | -------------
:hashId     | merkle hash of a file to download
:destinationFile     | target file name to which all pieces shall be saved when successfully downloaded


There also is one working always for first available hashId and storing result content into fixed file:

```
GET /asclient 
```

Peers and Trusted server endpoints are specified in appsettings.json

```
"MerkleFileClient": {
    "TrustedServer": "https://localhost:7000",
    "Peers": [
      "https://localhost:7001",
      "https://localhost:7002"
    ]
  }
 ```

#### Test scenario

It can be used to simulate distributed scenario if more instances of the application are started:
* One instance shall serve as trusted server - here connections to get list of hashes are routed
```
MerkleFileServer.Api.exe --file :fileFullName --urls "https://localhost:7000"
```
* Two more shall act as peers with already downloaded pieces
```
MerkleFileServer.Api.exe --file :fileFullName --urls "https://localhost:7001"
MerkleFileServer.Api.exe --file :fileFullName --urls "https://localhost:7001"
```

* Next client then could be configured with peers list (apsettings.json as above) 
```
MerkleFileServer.Api.exe --urls "https://localhost:7000"
```
Notice there is no file preload in that instance.

* Now this can be used to simulate download from distributed peers. That will initie the process of downloading pieces from all available peers spreading the load across few peers instances
```
GET /asclient
```
* After successfully downloading all pieces the result will be like:
```
{
  "destinationFileName": "C:\\MyPath\\destinationFile.png",
  "log": [
    "Downloading piece 0 from peer https://localhost:7001",
    "Downloading piece 3 from peer https://localhost:7001",
    "Downloading piece 4 from peer https://localhost:7001",
    "Downloading piece 7 from peer https://localhost:7001",
    "Downloading piece 9 from peer https://localhost:7001",
    "Downloading piece 10 from peer https://localhost:7001",
    "Downloading piece 12 from peer https://localhost:7001",
    "Downloading piece 14 from peer https://localhost:7001",
    "Downloading piece 16 from peer https://localhost:7001",
    "Downloading piece 1 from peer https://localhost:7002",
    "Downloading piece 2 from peer https://localhost:7002",
    "Downloading piece 5 from peer https://localhost:7002",
    "Downloading piece 6 from peer https://localhost:7002",
    "Downloading piece 8 from peer https://localhost:7002",
    "Downloading piece 11 from peer https://localhost:7002",
    "Downloading piece 13 from peer https://localhost:7002",
    "Downloading piece 15 from peer https://localhost:7002"
  ]
}
```
Notice that some pieces are downloaded from :7001 peer and others from :7002


# Design

Solution is organized following clean architecture guidlines, with domain as central place. In this case that is merkle tree details.

Application layer implements use case scenarios within services. 
_Grouped in feature folders, which at this point is folder per file but could be extened in future if more logis is added to scenarios._

ClientAddon is implemented as another project within that solution, sharing dependency on Domain and Infrastracture parts - <u>next step could be to split it out better, so that code for trusted server is isolated</u>.

### Merkle algorithm

For proofs calculation I've <u>added a marking side of the node</u> for the hash (left or right). This is realized just with prefixing 'L' or 'R' char to the proof hash hex. This makes later calculation easier and also limits probability of <u>second preimage attack</u> when certain hashes are expected on specific position.

### Next phase idea: read-ahead / hash-behind

When loading a file into pieces curentlly is requires to load all before calculating hashes and building a tree.

That part could be enhanced so that loading pieces is done in background thread. Then with each two pieces loaded, a tree could be updated - <u>paralleling hashing with reading and also limiting number of pieces hold in memory at same time</u>. Reading could be flusing into shared buffer listend by hash calculator.

That might be a must for bigger files, not only to make it faster but also not to append GBs into memory.

### What is missing

* There is no authentication nor authorization - auth tokens for trusted server could be added
* Exception handling - and few null checks
* Health checks, monitoring and logging has not been addressed
* More tests - focused on domain part tests, missing any integration tests, etc.
* More async methods
* Tested mainly the happy path, edge cases might fail
* Validating parameters has not been addressed fully so the process might fail with invlid ones
