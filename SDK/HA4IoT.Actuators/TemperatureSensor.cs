﻿using System;
using HA4IoT.Actuators.Contracts;
using HA4IoT.Hardware;
using HA4IoT.Networking;
using HA4IoT.Notifications;

namespace HA4IoT.Actuators
{
    public class TemperatureSensor : SingleValueSensorBase, ITemperatureSensor
    {
        public TemperatureSensor(string id, ISingleValueSensor sensor,
            IHttpRequestController httpApiController, INotificationHandler notificationHandler)
            : base(id, httpApiController, notificationHandler)
        {
            if (sensor == null) throw new ArgumentNullException(nameof(sensor));

            sensor.ValueChanged += (s, e) => UpdateValue(e.NewValue);
        }
    }
}