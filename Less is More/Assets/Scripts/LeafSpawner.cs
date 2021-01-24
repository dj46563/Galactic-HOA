﻿using System;
using System.Collections;
using System.Collections.Generic;
using Messages;
using UnityEngine;
using Random = UnityEngine.Random;

public class LeafSpawner : MonoBehaviour
{
    // TODO: use object pool
#pragma warning disable 0649
    [SerializeField] private GameObject leafPrefab;
#pragma warning restore 0649

    private Dictionary<int, Transform> _leafTransforms;
    private Dictionary<int, Vector2> _lastPositions;
    private int nextIdToUse = 0;

    private void Awake()
    {
        _lastPositions = new Dictionary<int, Vector2>();
        _leafTransforms = new Dictionary<int, Transform>();
    }

    public void SpawnLeafsRandomly(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomAngle = Random.Range(0, 2f * Mathf.PI);
            float randomRadius = Random.Range(0, 4.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            Vector2 position = PolarToRect(randomAngle, randomRadius);
            GameObject newLeaf = Instantiate(leafPrefab, position, randomRotation);

            LeafState leafState = new LeafState()
            {
                position = newLeaf.transform.position,
                rotation = newLeaf.transform.rotation
            };
            _leafTransforms[nextIdToUse] = newLeaf.transform;
            _lastPositions[nextIdToUse] = position;
            
            nextIdToUse++;
        }
    }

    public Dictionary<int, LeafState> GenerateLeafStates(bool onlySendDirty)
    {
        var leafStates = new Dictionary<int, LeafState>();
        foreach (var keyValue in _leafTransforms)
        {
            Transform leafTransform = keyValue.Value;
            int key = keyValue.Key;
            if (_lastPositions[key] != (Vector2) leafTransform.position || !onlySendDirty)
            {
                LeafState leafState = new LeafState()
                {
                    position = leafTransform.position,
                    rotation = leafTransform.rotation
                };
                leafStates[key] = leafState;
            }

            _lastPositions[key] = _leafTransforms[key].position;
        }

        return leafStates;
    }

    public GameObject SpawnLeaf(Vector2 position, Quaternion rotation)
    {
        GameObject newLeaf = Instantiate(leafPrefab, position, rotation);
        return newLeaf;
    }

    private Vector2 PolarToRect(float theta, float radius)
    {
        return new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
    }
}
