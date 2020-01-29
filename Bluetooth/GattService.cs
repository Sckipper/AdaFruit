using System;

namespace Medic
{
    /// <summary>
    /// Details about a specific GATT Service
    /// <seealso cref="https://www.bluetooth.com/specifications/gatt/services/"/>
    /// </summary>
    public class GattService
    {
        public static readonly String FloraSensorService = "00003000-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraAccelerometerCharacteristic = "00003001-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraMagnetometerCharacteristic = "00003002-0000-1000-8000-00805f9b34fb";
        public static readonly String FloraGyroscopeCharacteristic = "00003003-0000-1000-8000-00805f9b34fb";

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
