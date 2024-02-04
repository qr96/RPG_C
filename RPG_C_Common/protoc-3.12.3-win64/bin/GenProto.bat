protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../RPG_C_Server/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../RPG_C_Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../RPG_C_Server/Server/Packet"