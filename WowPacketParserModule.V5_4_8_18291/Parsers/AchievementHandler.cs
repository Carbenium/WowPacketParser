using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V5_4_8_18291.Parsers
{
    public static class AchievementHandler
    {
        [Parser(Opcode.SMSG_CRITERIA_UPDATE_ACCOUNT)]
        public static void HandleCriteriaUpdateAccount(Packet packet)
        {
            var counter = new byte[8];
            var accountId = new byte[8];

            counter[4] = packet.ReadBit();
            accountId[2] = packet.ReadBit();
            counter[2] = packet.ReadBit();
            accountId[4] = packet.ReadBit();
            counter[0] = packet.ReadBit();
            counter[5] = packet.ReadBit();
            accountId[3] = packet.ReadBit();
            counter[3] = packet.ReadBit();
            accountId[6] = packet.ReadBit();
            counter[6] = packet.ReadBit();
            accountId[1] = packet.ReadBit();
            accountId[7] = packet.ReadBit();
            counter[1] = packet.ReadBit();

            packet.ReadBits("Flags", 4); // some flag... & 1 -> delete

            accountId[5] = packet.ReadBit();
            counter[7] = packet.ReadBit();
            accountId[0] = packet.ReadBit();

            packet.ReadXORByte(accountId, 7);
            packet.ReadUInt32("Timer 2");
            packet.ReadInt32("Criteria ID");
            packet.ReadXORByte(counter, 7);
            packet.ReadUInt32("Timer 1");
            packet.ReadXORByte(accountId, 4);
            packet.ReadXORByte(accountId, 3);
            packet.ReadPackedTime("Time");
            packet.ReadXORByte(counter, 0);
            packet.ReadXORByte(counter, 1);
            packet.ReadXORByte(counter, 2);
            packet.ReadXORByte(counter, 3);
            packet.ReadXORByte(accountId, 1);
            packet.ReadXORByte(counter, 4);
            packet.ReadXORByte(counter, 5);
            packet.ReadXORByte(accountId, 5);
            packet.ReadXORByte(accountId, 2);
            packet.ReadXORByte(counter, 6);
            packet.ReadXORByte(accountId, 0);
            packet.ReadXORByte(accountId, 6);

            packet.WriteLine("Account: {0}", BitConverter.ToUInt64(accountId, 0));
            packet.WriteLine("Counter: {0}", BitConverter.ToInt64(counter, 0));
        }

        [Parser(Opcode.SMSG_CRITERIA_UPDATE_PLAYER)]
        public static void HandleCriteriaPlayer(Packet packet)
        {
            var guid = new byte[8];

            packet.StartBitStream(guid, 4, 6, 2, 3, 7, 1, 5, 0);

            packet.ReadXORByte(guid, 3);
            packet.ReadXORByte(guid, 6);
            packet.ReadXORByte(guid, 2);
            packet.ReadInt32("Criteria ID");
            packet.ReadInt32("Flags");
            packet.ReadXORByte(guid, 5);
            packet.ReadXORByte(guid, 1);
            packet.ReadPackedTime("Time");
            packet.ReadXORByte(guid, 4);
            packet.ReadInt32("Timer 1");
            packet.ReadInt32("Timer 2");
            packet.ReadXORByte(guid, 7);
            packet.ReadXORByte(guid, 0);
            packet.ReadInt64("Counter");

            packet.WriteGuid("Guid", guid);
        }

        [Parser(Opcode.SMSG_ALL_ACHIEVEMENT_DATA_PLAYER)]
        public static void HandleAllAchievementDataPlayer(Packet packet)
        {
            var bits20 = packet.ReadBits("Criteria count", 19);

            var counter = new byte[bits20][];
            var guid2 = new byte[bits20][];
            var flags = new byte[bits20];

            for (var i = 0; i < bits20; ++i)
            {
                counter[i] = new byte[8];
                guid2[i] = new byte[8];

                guid2[i][3] = packet.ReadBit();
                counter[i][3] = packet.ReadBit();
                counter[i][6] = packet.ReadBit();
                guid2[i][0] = packet.ReadBit();
                counter[i][7] = packet.ReadBit();
                guid2[i][1] = packet.ReadBit();
                guid2[i][5] = packet.ReadBit();
                counter[i][2] = packet.ReadBit();
                counter[i][1] = packet.ReadBit();
                guid2[i][7] = packet.ReadBit();
                counter[i][4] = packet.ReadBit();
                counter[i][0] = packet.ReadBit();
                guid2[i][2] = packet.ReadBit();
                counter[i][5] = packet.ReadBit();
                guid2[i][4] = packet.ReadBit();
                flags[i] = (byte)(packet.ReadBits(4) & 0xFFu);
                guid2[i][6] = packet.ReadBit();
            }

            var bits10 = packet.ReadBits("Achievement count", 20);

            var guid1 = new byte[bits10][];
            for (var i = 0; i < bits10; ++i)
            {
                guid1[i] = new byte[8];
                packet.StartBitStream(guid1[i], 0, 7, 1, 5, 2, 4, 6, 3);
            }

            for (var i = 0; i < bits10; ++i)
            {
                packet.ReadUInt32("Achievement Id", i);
                packet.ReadInt32("Realm Id", i);
                packet.ReadXORByte(guid1[i], 5);
                packet.ReadXORByte(guid1[i], 7);
                packet.ReadInt32("Realm Id", i);
                packet.ReadPackedTime("Time", i);
                packet.ReadXORByte(guid1[i], 0);
                packet.ReadXORByte(guid1[i], 4);
                packet.ReadXORByte(guid1[i], 1);
                packet.ReadXORByte(guid1[i], 6);
                packet.ReadXORByte(guid1[i], 2);
                packet.ReadXORByte(guid1[i], 3);

                packet.WriteGuid("Guid3", guid1[i], i);
            }

            for (var i = 0; i < bits20; ++i)
            {
                packet.ReadXORByte(guid2[i], 7);
                packet.ReadUInt32("Timer 1", i);
                packet.ReadXORByte(guid2[i], 6);
                packet.ReadXORByte(counter[i], 1);
                packet.ReadInt32("Criteria ID", i);
                packet.ReadXORByte(guid2[i], 4);
                packet.ReadXORByte(counter[i], 0);
                packet.ReadXORByte(counter[i], 4);
                packet.ReadXORByte(counter[i], 6);
                packet.ReadXORByte(guid2[i], 1);
                packet.ReadXORByte(guid2[i], 5);
                packet.ReadXORByte(counter[i], 7);
                packet.ReadXORByte(counter[i], 2);
                packet.ReadXORByte(guid2[i], 2);
                packet.ReadXORByte(guid2[i], 0);
                packet.ReadXORByte(counter[i], 3);
                packet.ReadUInt32("Timer 2", i);
                packet.ReadXORByte(guid2[i], 3);
                packet.ReadXORByte(counter[i], 5);
                packet.ReadPackedTime("Time", i);

                packet.WriteGuid("Guid2", guid2[i], i);

                packet.WriteLine("[{0}] Criteria Flags: {1}", i, flags[i]);
                packet.WriteLine("[{0}] Criteria Counter: {1}", i, BitConverter.ToUInt64(guid2[i], 0)); // guid and counter is switched, was lazy to remake it
                packet.WriteGuid("Criteria GUID", counter[i], i);
            }
        }
    }
}
