using System;

namespace Medic
{
    /// <summary>
    /// Details about a specific GATT Service
    /// <seealso cref="https://www.bluetooth.com/specifications/gatt/services/"/>
    /// </summary>
    public class GattService
    {
        public static readonly String FloraAccelerometerService = "00003000-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraAccelerometerCharacteristicX = "00003001-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraAccelerometerCharacteristicY = "00003002-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraAccelerometerCharacteristicZ = "00003003-0000-1000-8000-00805f9b34fb";

        public static readonly String FloraGyroscopeService = "00003004-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraGyroscopeCharacteristicX = "00003005-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraGyroscopeCharacteristicY = "00003006-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraGyroscopeCharacteristicZ = "00003007-0000-1000-8000-00805f9b34fb";

        public static readonly String FloraMagnetometerService = "00003008-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraMagnetometerCharacteristicX = "00003009-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraMagnetometerCharacteristicY = "00003010-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraMagnetometerCharacteristicZ = "00003011-0000-1000-8000-00805f9b34fb";

        #region Public Properties

        /// <summary>
        /// The human-readable name for the service
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The uniform identifier that is unique to this service
        /// </summary>
        public string UniformTypeIdentifier { get; }

        /// <summary>
        /// The 16-bit assigned number for this service.
        /// The Bluetooth GATT Service UUID contains this.
        /// </summary>
        public ushort AssignedNumber { get; }

        /// <summary>
        /// The type of specification that this service is.
        /// <seealso cref="https://www.bluetooth.com/specifications/gatt/"/>
        /// </summary>
        public string ProfileSpecification { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public GattService(string name, string uniformIdenfier, ushort assignedNumber, string profileSpecification)
        {
            Name = name;
            UniformTypeIdentifier = uniformIdenfier;
            AssignedNumber = assignedNumber;
            ProfileSpecification = profileSpecification;
        }

        #endregion
    }
}
