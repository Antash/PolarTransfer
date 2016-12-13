using System;
using System.Collections.Generic;

namespace SportTrackerManager.Core
{
    internal class PolarFlowConverter : IValueConverter
    {
        private static readonly Dictionary<int, Excercise> sports = new Dictionary<int, Excercise>()
        {
            { 1, Excercise.Running },
        };

        public double GetDistance(string text)
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetDuration(string text)
        {
            throw new NotImplementedException();
        }

        public Excercise GetExcerciseType(string text)
        {
            switch (int.Parse(text))
            {
                case 1:
                    return Excercise.Running;
                case 105:
                case 23:
                case 103:
                    return Excercise.Swimming;
                case 2:
                case 38:
                    return Excercise.Cycling;
                case 18:
                    return Excercise.IndoorCycling;
                case 15:
                    return Excercise.OPA;
                case 6:
                    return Excercise.ScateSkiing;
                case 3:
                    return Excercise.Walking;
                default:
                    return Excercise.OtherSport;
            }
            //return sports[int.Parse(text)];
        }

        public DateTime GetStartDateTime(string text)
        {
            throw new NotImplementedException();
        }
    }
}
/*TODO use all sports
 *   <option value="2" >Cycling</option>
        
            <option value="83" >Other indoor</option>
        
            <option value="16" >Other outdoor</option>
        
            <option value="103" >Pool swimming</option>
        
            <option value="1" selected>Running</option>
        
            <option value="6" >Skiing</option>
        
            <option value="3" >Walking</option>
        
            <option value="61" >Aerobics</option>
        
            <option value="62" >Aqua fitness</option>
        
            <option value="113" >Backcountry skiing</option>
        
            <option value="14" >Badminton</option>
        
            <option value="122" >Ballet</option>
        
            <option value="125" >Ballroom</option>
        
            <option value="42" >Baseball</option>
        
            <option value="41" >Basketball</option>
        
            <option value="49" >Beach volley</option>
        
            <option value="87" >Biathlon</option>
        
            <option value="64" >Body&amp;Mind</option>
        
            <option value="58" >Bootcamp</option>
        
            <option value="109" >Boxing</option>
        
            <option value="96" >Canoeing</option>
        
            <option value="20" >Circuit training</option>
        
            <option value="25" >Classic XC skiing</option>
        
            <option value="60" >Classic roller skiing</option>
        
            <option value="94" >Climbing (indoor)</option>
        
            <option value="126" >Core</option>
        
            <option value="40" >Cricket</option>
        
            <option value="55" >Cross-trainer</option>
        
            <option value="34" >Crossfit</option>
        
            <option value="52" >Dancing</option>
        
            <option value="90" >Disc golf</option>
        
            <option value="7" >Downhill skiing</option>
        
            <option value="44" >Field hockey</option>
        
            <option value="104" >Finnish baseball</option>
        
            <option value="67" >Fitness dancing</option>
        
            <option value="56" >Fitness martial arts</option>
        
            <option value="51" >Floorball</option>
        
            <option value="47" >Football</option>
        
            <option value="24" >Freestyle XC skiing</option>
        
            <option value="59" >Freestyle roller skiing</option>
        
            <option value="57" >Functional training</option>
        
            <option value="50" >Futsal</option>
        
            <option value="35" >Golf</option>
        
            <option value="32" >Group exercise</option>
        
            <option value="114" >Gymnastics</option>
        
            <option value="48" >Handball</option>
        
            <option value="11" >Hiking</option>
        
            <option value="46" >Ice hockey</option>
        
            <option value="28" >Ice skating</option>
        
            <option value="18" >Indoor cycling</option>
        
            <option value="117" >Indoor rowing</option>
        
            <option value="29" >Inline skating</option>
        
            <option value="123" >Jazz</option>
        
            <option value="4" >Jogging</option>
        
            <option value="115" >Judo</option>
        
            <option value="95" >Kayaking</option>
        
            <option value="110" >Kickboxing</option>
        
            <option value="100" >Kitesurfing</option>
        
            <option value="129" >LES MILLS BODYATTACK</option>
        
            <option value="140" >LES MILLS BODYBALANCE</option>
        
            <option value="130" >LES MILLS BODYCOMBAT</option>
        
            <option value="136" >LES MILLS BODYJAM</option>
        
            <option value="128" >LES MILLS BODYPUMP</option>
        
            <option value="137" >LES MILLS BODYSTEP</option>
        
            <option value="139" >LES MILLS BODYVIVE</option>
        
            <option value="142" >LES MILLS CXWORX</option>
        
            <option value="131" >LES MILLS GRIT Cardio</option>
        
            <option value="133" >LES MILLS GRIT Plyo</option>
        
            <option value="132" >LES MILLS GRIT Strength</option>
        
            <option value="135" >LES MILLS RPM</option>
        
            <option value="134" >LES MILLS SH&#x27;BAM</option>
        
            <option value="138" >LES MILLS SPRINT</option>
        
            <option value="141" >LES MILLS THE TRIP</option>
        
            <option value="120" >Latin</option>
        
            <option value="111" >Mobility (dynamic)</option>
        
            <option value="127" >Mobility (static)</option>
        
            <option value="124" >Modern</option>
        
            <option value="86" >Mountain bike orienteering</option>
        
            <option value="5" >Mountain biking</option>
        
            <option value="9" >Nordic walking</option>
        
            <option value="105" >Open water swimming</option>
        
            <option value="84" >Orienteering</option>
        
            <option value="65" >Pilates</option>
        
            <option value="54" >Riding</option>
        
            <option value="38" >Road cycling</option>
        
            <option value="19" >Road running</option>
        
            <option value="30" >Roller skating</option>
        
            <option value="8" >Rowing</option>
        
            <option value="43" >Rugby</option>
        
            <option value="88" >Sailing</option>
        
            <option value="121" >Show</option>
        
            <option value="10" >Skating</option>
        
            <option value="85" >Ski orienteering</option>
        
            <option value="22" >Snowboarding</option>
        
            <option value="116" >Snowshoe trekking</option>
        
            <option value="39" >Soccer</option>
        
            <option value="118" >Spinning</option>
        
            <option value="13" >Squash</option>
        
            <option value="63" >Step workout</option>
        
            <option value="119" >Street</option>
        
            <option value="15" >Strength training</option>
        
            <option value="66" >Stretching</option>
        
            <option value="102" >Surfing</option>
        
            <option value="23" >Swimming</option>
        
            <option value="91" >Table tennis</option>
        
            <option value="112" >Telemark skiing</option>
        
            <option value="12" >Tennis</option>
        
            <option value="36" >Track&amp;field running</option>
        
            <option value="27" >Trail running</option>
        
            <option value="17" >Treadmill running</option>
        
            <option value="53" >Trotting</option>
        
            <option value="92" >Ultra running</option>
        
            <option value="45" >Volleyball</option>
        
            <option value="107" >Wakeboarding</option>
        
            <option value="108" >Water skiing</option>
        
            <option value="89" >Wheelchair racing</option>
        
            <option value="101" >Windsurfing</option>
        
            <option value="33" >Yoga</option>
 * */
