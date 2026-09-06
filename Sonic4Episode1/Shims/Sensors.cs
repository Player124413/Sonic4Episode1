#if ANDROID || IOS || WASM
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#if ANDROID
using Android.Content;
using Android.Hardware;
#elif IOS
using CoreMotion;
using Foundation;
#elif WASM
using System.Runtime.InteropServices.JavaScript;
#endif

namespace Microsoft.Devices.Sensors
{
    public struct AccelerometerReading
    {
        public Vector3 Acceleration;
    }

    public class SensorReadingEventArgs<T> : EventArgs
    {
        public readonly T SensorReading;

        internal SensorReadingEventArgs(T reading)
        {
            SensorReading = reading;
        }
    }

    public class AccelerometerFailedException : Exception
    {
    }

#if ANDROID

    // amIPhoneAccelerate was written against the iPhone convention, which is g and points the
    // opposite way to Android's. SensorManager reports m/s^2 in the natural device frame, and does
    // not rotate with the display, so the frame itself already matches
    public partial class Accelerometer : Java.Lang.Object, ISensorEventListener
    {
        private readonly SensorManager _manager;
        private readonly Sensor _sensor;

        public Accelerometer()
        {
            _manager = (SensorManager)Sonic4Episode1.Android.Sonic4Activity.Current
                ?.GetSystemService(Context.SensorService);
            _sensor = _manager?.GetDefaultSensor(SensorType.Accelerometer);

            if (_sensor == null)
                throw new AccelerometerFailedException();
        }

        public TimeSpan TimeBetweenUpdates { get; set; } = TimeSpan.FromSeconds(1.0 / 30.0);
        public event EventHandler<SensorReadingEventArgs<AccelerometerReading>> CurrentValueChanged;

        public void Start()
        {
            // the SensorDelay overload also takes a raw microsecond period, which is the only way
            // to honour TimeBetweenUpdates rather than snapping to one of the four named rates
            var microseconds = (int)(TimeBetweenUpdates.TotalMilliseconds * 1000);
            _manager.RegisterListener(this, _sensor, (SensorDelay)microseconds);
        }

        public void Stop()
        {
            _manager.UnregisterListener(this, _sensor);
        }

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Values == null || e.Values.Count < 3)
                return;

            const float scale = -1f / 9.80665f;
            var acceleration = new Vector3(e.Values[0] * scale, e.Values[1] * scale, e.Values[2] * scale);

            CurrentValueChanged?.Invoke(this,
                new SensorReadingEventArgs<AccelerometerReading>(
                    new AccelerometerReading { Acceleration = acceleration }));
        }
    }

#elif IOS

    // CoreMotion already reports g in the frame amIPhoneAccelerate was written against, so unlike
    // the Android shim there is nothing to convert
    public partial class Accelerometer
    {
        private readonly CMMotionManager _motion = new CMMotionManager();

        public Accelerometer()
        {
            if (!_motion.AccelerometerAvailable)
                throw new AccelerometerFailedException();
        }

        public TimeSpan TimeBetweenUpdates { get; set; } = TimeSpan.FromSeconds(1.0 / 30.0);
        public event EventHandler<SensorReadingEventArgs<AccelerometerReading>> CurrentValueChanged;

        public void Start()
        {
            _motion.AccelerometerUpdateInterval = TimeBetweenUpdates.TotalSeconds;
            _motion.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue ?? NSOperationQueue.MainQueue, Handle);
        }

        public void Stop()
        {
            _motion.StopAccelerometerUpdates();
        }

        private void Handle(CMAccelerometerData data, NSError error)
        {
            if (data == null || error != null)
                return;

            var acceleration = new Vector3(
                (float)data.Acceleration.X,
                (float)data.Acceleration.Y,
                (float)data.Acceleration.Z);

            CurrentValueChanged?.Invoke(this,
                new SensorReadingEventArgs<AccelerometerReading>(
                    new AccelerometerReading { Acceleration = acceleration }));
        }
    }

#elif WASM

    public partial class Accelerometer
    {
        private static readonly List<Accelerometer> Listening = [];

        public TimeSpan TimeBetweenUpdates { get; set; }
        public event EventHandler<SensorReadingEventArgs<AccelerometerReading>> CurrentValueChanged;

        public void Start()
        {
            Listening.Add(this);
        }

        public void Stop()
        {
            Listening.Remove(this);
        }

        [JSExport]
        internal static void Invoke(float x, float y, float z)
        {
            var reading = new AccelerometerReading { Acceleration = new Vector3(x, y, z) };

            foreach (var listener in Listening)
            {
                listener.CurrentValueChanged?.Invoke(listener,
                    new SensorReadingEventArgs<AccelerometerReading>(reading));
            }
        }
    }

#endif
}
#endif
