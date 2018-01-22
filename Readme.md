AppXtivate
==========

AppXtivate is a small executable to start AppX (Windows Store) packaged applications from the command line.

Why ?
-----

Windows Store applications are started in a sandbox and don't actually have executables on disk, and even for normal applications wrapped in AppX that have an executable it can't actually be started.

It is a problem in case you need to start one from a batch file or from another Win32 program.

Usage
-----

```batch
AppXtivate run AppUserModelId [...args]
```

For example with Affinity Photo:

```batch
AppXtivate run SerifEuropeLtd.AffinityPhoto_844sdzfcmm7k0!SerifEuropeLtd.AffinityPhoto "C:\temp\picture.png"
```