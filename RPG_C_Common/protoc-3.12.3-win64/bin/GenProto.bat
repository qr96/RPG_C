protoc.exe -I=./ --csharp_out=./ ./Protocol.proto 
IF ERRORLEVEL 1 PAUSE

START ../../../RPG_B_Server/PacketGenerator/bin/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../RPG_B_Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../RPG_B_Server/Server/Packet"