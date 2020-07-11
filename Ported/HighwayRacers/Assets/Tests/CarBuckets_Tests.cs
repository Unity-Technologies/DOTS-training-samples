using System;
using System.Collections;
using System.Collections.Generic;
using HighwayRacer;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Random = Unity.Mathematics.Random;

namespace Tests
{
    public class CarBuckets_Tests
    {
        [Test]
        public void CarBuckets_FindCarInBucket()
        {
            const float maxSpeed = 100.0f;
            const int nLanes = 4;

            const int n = 1000;

            var buckets = new CarBuckets(1, n);
            Assert.IsTrue(buckets.IsCreated, "buckets not created");

            // add random cars
            var rand = new Random(100); // for repeatability just pick hardcoded number

            for (int i = 0; i < n; i++)
            {
                buckets.AddCar(
                    new Segment() {Val = 0}, // cars all will belong to same segment
                    new TrackPos() {Val = rand.NextFloat()},
                    new Speed() {Val = rand.NextFloat() * maxSpeed},
                    new Lane() {Val = (byte) rand.NextInt(nLanes)}
                );
            }

            buckets.Sort();

            var bucket = buckets.GetCars(0);

            for (int i = 0; i < n; i++)
            {
                var car = bucket[i];
                var index = CarUtil.findInBucket(bucket, car.Pos, car.Lane);
                Assert.AreEqual(i, index);
            }
        }

        [Test]
        public void CarBuckets_CarsSorted()
        {
            const float maxSpeed = 100.0f;
            const int nLanes = 4;

            const int n = 1000;

            var buckets = new CarBuckets(1, n);
            Assert.IsTrue(buckets.IsCreated, "buckets not created");

            // add random cars
            var rand = new Random(100); // for repeatability just pick hardcoded number

            for (int i = 0; i < n; i++)
            {
                buckets.AddCar(
                    new Segment() {Val = 0}, // cars all will belong to same segment
                    new TrackPos() {Val = rand.NextFloat()},
                    new Speed() {Val = rand.NextFloat() * maxSpeed},
                    new Lane() {Val = (byte) rand.NextInt(nLanes)}
                );
            }

            buckets.Sort();

            var bucket = buckets.GetCars(0);

            var lastCar = new Car()
            {
                Pos = 0.0f,
                Lane = 0,
            };

            for (int i = 0; i < n; i++)
            {
                var car = bucket[i];
                //Debug.Log("i: " + i + " pos: " + car.Pos + " lane: " + car.Lane);
                if (car.Pos == lastCar.Pos)
                {
                    Debug.Log("Equal POS i: " + i + " pos: " + car.Pos + " lane: " + car.Lane);
                    Assert.True(car.Lane >= lastCar.Lane, "cars with same pos: improper lane order");
                }
                else
                {
                    Assert.True(car.Pos > lastCar.Pos, "car with greater pos sorted after car with lesser pos");
                }
                lastCar = car;
            }
        }
    }
}