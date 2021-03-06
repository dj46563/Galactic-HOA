﻿using System.Collections;
using System.Collections.Generic;
using NetStack.Quantization;
using NetStack.Serialization;
using UnityEngine;

namespace Messages
{
    public struct PeerState : BitSerializable
    {
        public Vector2 position;
        public string currentAnimation;
        public bool spriteFlipped;
        public bool isPlaying;
        public bool pressingSpace;
        public Vector2 mouseDir;
        public string displayName;
        public ushort score;
        public ushort roundsPlayed;
        public bool isColorsDirty;
        public ushort headColorCode;
        public ushort bodyColorCode;
        public ushort feetColorCode;


        public void Serialize(ref BitBuffer data)
        {
            if (currentAnimation == null)
                currentAnimation = "idle";
            
            QuantizedVector2 qPosition = BoundedRange.Quantize(position, Constants.WORLD_BOUNDS);
            QuantizedVector2 qMouseDir = BoundedRange.Quantize(mouseDir, Constants.MOUSEDIR_BOUNDS);

            data.AddUInt(qPosition.x)
                .AddUInt(qPosition.y)
                .AddUInt(qMouseDir.x)
                .AddUInt(qMouseDir.y)
                .AddString(currentAnimation)
                .AddBool(spriteFlipped)
                .AddBool(isPlaying)
                .AddBool(pressingSpace)
                .AddString(displayName)
                .AddUShort(score)
                .AddUShort(roundsPlayed);
            
            data.AddBool(isColorsDirty);
            if (isColorsDirty)
            {
                data.AddUShort(headColorCode)
                    .AddUShort(bodyColorCode)
                    .AddUShort(feetColorCode);
            }
        }

        public void Deserialize(ref BitBuffer data)
        {
            QuantizedVector2 qPosition = new QuantizedVector2(data.ReadUInt(), data.ReadUInt());
            QuantizedVector2 qMouseDir = new QuantizedVector2(data.ReadUInt(), data.ReadUInt());
            position = BoundedRange.Dequantize(qPosition, Constants.WORLD_BOUNDS);
            mouseDir = BoundedRange.Dequantize(qMouseDir, Constants.MOUSEDIR_BOUNDS);
            currentAnimation = data.ReadString();
            spriteFlipped = data.ReadBool();
            isPlaying = data.ReadBool();
            pressingSpace = data.ReadBool();
            displayName = data.ReadString();
            score = data.ReadUShort();
            roundsPlayed = data.ReadUShort();

            isColorsDirty = data.ReadBool();
            if (isColorsDirty)
            {
                headColorCode = data.ReadUShort();
                bodyColorCode = data.ReadUShort();
                feetColorCode = data.ReadUShort();
            }
        }
    }

    public struct LeafState : BitSerializable
    {
        public Vector2 position;
        public Quaternion rotation;
        public float heightInAir;
        public bool IsNew;
        
        public void Serialize(ref BitBuffer data)
        {
            QuantizedVector2 qPosition = BoundedRange.Quantize(position, Constants.WORLD_BOUNDS);
            QuantizedQuaternion qRotation = SmallestThree.Quantize(rotation);
            ushort qHeightInAir = HalfPrecision.Quantize(heightInAir);

            data.AddUInt(qPosition.x)
                .AddUInt(qPosition.y)
                .AddUInt(qRotation.m)
                .AddUInt(qRotation.a)
                .AddUInt(qRotation.b)
                .AddUInt(qRotation.c)
                .AddUShort(qHeightInAir)
                .AddBool(IsNew);
        }

        public void Deserialize(ref BitBuffer data)
        {
            QuantizedVector2 qPosition = new QuantizedVector2(data.ReadUInt(), data.ReadUInt());
            QuantizedQuaternion qRotation = new QuantizedQuaternion(data.ReadUInt(), data.ReadUInt(),data.ReadUInt(), data.ReadUInt());
            ushort qHeightInAir = data.ReadUShort();

            position = BoundedRange.Dequantize(qPosition, Constants.WORLD_BOUNDS);
            rotation = SmallestThree.Dequantize(qRotation);
            heightInAir = HalfPrecision.Dequantize(qHeightInAir);
            IsNew = data.ReadBool();
        }
    }
    
    public struct PeerStates : BitSerializable
    {
        public const ushort id = 3;
        public Dictionary<int, PeerState> States;
        public Dictionary<int, LeafState> Leafs;
        public List<ushort> SegmentLeafCounts;

        public void Serialize(ref BitBuffer data)
        {
            data.AddUShort(id);
            
            data.AddInt(States.Count);
            foreach (var peerState in States)
            {
                data.AddInt(peerState.Key);
                peerState.Value.Serialize(ref data);
            }

            data.AddInt(Leafs.Count);
            foreach (var leaf in Leafs)
            {
                data.AddInt(leaf.Key);
                leaf.Value.Serialize(ref data);
            }

            data.AddShort((short)SegmentLeafCounts.Count);
            foreach (var segmentLeafCount in SegmentLeafCounts)
            {
                data.AddUShort(segmentLeafCount);
            }
        }

        public void Deserialize(ref BitBuffer data)
        {
            data.ReadUShort();
            
            int count = data.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int peerId = data.ReadInt();
                PeerState peerState = new PeerState();
                peerState.Deserialize(ref data);
                States[peerId] = peerState;
            }

            int leafCount = data.ReadInt();
            for (int i = 0; i < leafCount; i++)
            {
                int leafId = data.ReadInt();
                LeafState leafState = new LeafState();
                leafState.Deserialize(ref data);
                Leafs[leafId] = leafState;
            }

            int segmentCount = data.ReadShort();
            SegmentLeafCounts = new List<ushort>();
            for (int i = 0; i < segmentCount; i++)
            {
                SegmentLeafCounts.Add(data.ReadUShort());
            }
        }
    }
}

