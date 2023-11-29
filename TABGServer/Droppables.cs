﻿namespace TABGCommunityServer
{
    internal class Droppables
    {
        public static byte[] ClientRequestDrop(BinaryReader binaryReader)
        {
            // player
            var playerIndex = binaryReader.ReadByte();
            // item id
            var itemID = binaryReader.ReadInt32();
            // count of items
            var itemCount = binaryReader.ReadInt32();

            // location
            var x = binaryReader.ReadSingle();
            var y = binaryReader.ReadSingle();
            var z = binaryReader.ReadSingle();

            int networkID = WeaponConcurrencyHandler.CurrentID;
            Weapon weapon = new Weapon(networkID, itemID, itemCount, (x, y, z));
            WeaponConcurrencyHandler.SpawnWeapon(weapon);

            return SendItemDropPacket(networkID, itemID, itemCount, (x, y, z));
        }

        public static byte[] SendItemDropPacket(int index, int type, int count, (float x, float y, float z) loc)
        {
            byte[] sendByte = new byte[512];
            using (MemoryStream writerMemoryStream = new MemoryStream(sendByte))
            {
                using (BinaryWriter binaryWriterStream = new BinaryWriter(writerMemoryStream))
                {
                    // index (doesn't seem to serve a purpose)
                    binaryWriterStream.Write((Int32)index);
                    // type
                    binaryWriterStream.Write(type);
                    // quantity
                    binaryWriterStream.Write(count);

                    // location
                    binaryWriterStream.Write(loc.x);
                    binaryWriterStream.Write(loc.y);
                    binaryWriterStream.Write(loc.z);
                }
            }
            return sendByte;
        }

        public static byte[] ClientRequestPickUp(BinaryReader binaryReader)
        {
            // player
            var playerIndex = binaryReader.ReadByte();
            // item network index
            var netIndex = binaryReader.ReadInt32();
            // item slot of player
            var itemSlot = binaryReader.ReadByte();

            Weapon weapon = WeaponConcurrencyHandler.WeaponDB[netIndex];

            // clean up DB
            WeaponConcurrencyHandler.RemoveWeapon(weapon);

            return SendWeaponPickUpAcceptedPacket(playerIndex, netIndex, weapon.Type, weapon.Count, itemSlot);
        }

        public static byte[] SendWeaponPickUpAcceptedPacket(byte playerID, int networkIndex, int weaponID, int quantity, byte slot)
        {
            byte[] sendByte = new byte[512];
            using (MemoryStream writerMemoryStream = new MemoryStream(sendByte))
            {
                using (BinaryWriter binaryWriterStream = new BinaryWriter(writerMemoryStream))
                {
                    // player id
                    binaryWriterStream.Write(playerID);
                    // network index of the gun
                    binaryWriterStream.Write(networkIndex);
                    // weapon index in the game (id)
                    binaryWriterStream.Write(weaponID);
                    // quantity
                    binaryWriterStream.Write(quantity);
                    // slot
                    binaryWriterStream.Write(slot);
                }
            }
            return sendByte;
        }
    }
}