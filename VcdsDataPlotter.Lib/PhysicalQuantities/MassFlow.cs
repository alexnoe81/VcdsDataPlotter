using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.PhysicalQuantities
{
    public struct MassFlow
    {
        private MassFlow(double kgph) => this.kgph = kgph;
        public static MassFlow FromKgph(double kgph) => new MassFlow(kgph);
        public static MassFlow operator +(MassFlow left, MassFlow right) => FromKgph(left.kgph + right.kgph);
        public static MassFlow operator -(MassFlow left, MassFlow right) => FromKgph(left.kgph - right.kgph);
        public static MassFlow operator *(MassFlow left, double scalar) => FromKgph(left.kgph * scalar);
        public static MassFlow operator *(double scalar, MassFlow right) => FromKgph(right.kgph * scalar);
        public static MassFlow operator /(MassFlow left, double scalar) => FromKgph(left.kgph * 1.0/scalar);

        public double KilogramsPerHour => this.kgph;

        /// <summary>
        /// Raw value in kilograms per hour. We could also use grams per second or whatever....
        /// </summary>
        private double kgph;
    }
}
