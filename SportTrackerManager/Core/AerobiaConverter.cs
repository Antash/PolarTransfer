using System;
using System.Globalization;

namespace SportTrackerManager.Core
{
    internal class AerobiaConverter : IValueConverter
    {
        //TODO ENG
        private const string TimeSpanFormatRu = @"h\ч\:mm\м\:ss\с";
        private const string TimeSpanFormatHRu = @"mm\м\:ss\с";
        private const string TimeSpanFormatEn = @"mm\m\:ss\s";
        private const string TimeSpanFormatHEn = @"h\h\:mm\m\:ss\s";
        private const string DateTimeFormatRu = "d MMM'.' yyyy'г, в' HH:mm";
        private const string DateTimeFormatEn = "";

        public double GetDistance(string text)
        {
            var dist = text.Replace("км", string.Empty).Trim();
            return double.Parse(dist, CultureInfo.InvariantCulture);
        }

        public TimeSpan GetDuration(string text)
        {
            TimeSpan duration;
            if (!TimeSpan.TryParseExact(text.Trim(), TimeSpanFormatRu, CultureInfo.GetCultureInfo("ru-RU"), out duration))
            {
                return TimeSpan.ParseExact(text.Trim(), TimeSpanFormatHRu, CultureInfo.GetCultureInfo("ru-RU"));
            }
            return duration;
        }

        public Excercise GetExcerciseType(string text)
        {
            //TODO extend
            switch (text.ToLower())
            {
                case "бег":
                    return Excercise.Running;
                case "офп":
                    return Excercise.OPA;
                case "плавание":
                    return Excercise.Swimming;
                case "прогулочный велосипед":
                case "велоспорт":
                    return Excercise.Cycling;
                case "триатлон":
                    return Excercise.Triathlon;
                case "велотренажер":
                    return Excercise.IndoorCycling;
                case "прогулка":
                    return Excercise.Walking;
                case "тренажерный зал":
                    return Excercise.Gym;
                case "лыжи коньковый ход":
                    return Excercise.ScateSkiing;
                case "лыжи классический ход":
                    return Excercise.ClassicSkiing;
                default:
                    return Excercise.OtherSport;
            }
        }

        public DateTime GetStartDateTime(string text)
        {
            return DateTime.ParseExact(PrepareDate(text), DateTimeFormatRu, CultureInfo.GetCultureInfo("ru-RU"));
        }

        private string PrepareDate(string text)
        {
            return text.Trim()
                .Replace("нояб", "ноя")
                .Replace("мая", "май.")
                .Replace("июня", "июн.")
                .Replace("июля", "июл.")
                .Replace("сент", "сен");
        }
    }
}
/*
<option value = "1" data-gray="/uploads/sports_small/1_2_cyclingsport_g.png?1346232077" data-white="/uploads/sports_small/1_2_cyclingsport_w_white.png?1346232077" data-distance="true">Велоспорт</option>
<option value = "2" selected="selected" data-gray="/uploads/sports_small/2_4_2_running_g.png?1346240783" data-white="/uploads/sports_small/2_4_2_running_w_white.png?1346240783" data-distance="true">Бег</option>
<option value = "3" data-gray="/uploads/sports_small/3_5_skiing_g.png?1346232195" data-white="/uploads/sports_small/3_5_skiing_w_white.png?1346232227" data-distance="true">Лыжи коньковый ход</option>
<option value = "6" data-gray="/uploads/sports_small/6_1_cyclingtrans_g.png?1346232118" data-white="/uploads/sports_small/6_1_cyclingtrans_w_white.png?1346232135" data-distance="true">Прогулочный велосипед</option>
  <option value = "19" data-gray= "/uploads/sports_small/19_8_other_g.png?1346240818" data-white= "/uploads/sports_small/19_8_other_w_white.png?1346240818" data-distance= "true" > Прогулка </ option >
  < option value= "21" data-gray= "/uploads/sports_small/21_7_swimming_g.png?1346232244" data-white= "/uploads/sports_small/21_7_swimming_w_white.png?1346232244" data-distance= "true" > Плавание </ option >
  < option value= "22" data-gray= "/uploads/sports_small/22_large_(14).png?1353666717" data-white= "/uploads/sports_small/22_large_(15)_white.png?1353666718" data-distance= "true" > Велотренажер </ option >
  < option value= "51" data-gray= "/uploads/sports_small/51_25_volleyball_g.png?1355335423" data-white= "/uploads/sports_small/51_25_volleyball_w_white.png?1355335423" data-distance= "false" > Волейбол пляжный</option>
  <option value = "53" data-gray= "/uploads/sports_small/53_31_basketball_g.png?1364125262" data-white= "/uploads/sports_small/53_31_basketball_w_white.png?1364125262" data-distance= "false" > Баскетбол </ option >
  < option value= "54" data-gray= "/uploads/sports_small/54_large_(5).png?1352893562" data-white= "/uploads/sports_small/54_large_(7)_white.png?1352893562" data-distance= "false" > Тренажерный зал</option>
  <option value = "56" data-gray= "/uploads/sports_small/56_3_mountbiking_g.png?1346232148" data-white= "/uploads/sports_small/56_3_mountbiking_w_white.png?1346232148" data-distance= "true" > Горный велоспорт</option>
  <option value = "65" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Триатлон </ option >
  < option value= "72" data-gray= "/uploads/sports_small/72_49_black_tr.png?1391097971" data-white= "/uploads/sports_small/72_49_tr_white.png?1391097971" data-distance= "false" > ОФП </ option >
  < option value= "83" data-gray= "/uploads/sports_small/83_3_5_skiing_g_thumbnail.png?1388155576" data-white= "/uploads/sports_small/83_3_5_skiing_w_white_thumbnail_(1)_white.png?1388155576" data-distance= "true" > Лыжи классический ход</option>
<option value = "" disabled= "true" > -------------</ option >
  < option value= "77" data-gray= "/uploads/sports_small/77_36_black.png?1390913277" data-white= "/uploads/sports_small/77_36_tr_white.png?1390913277" data-distance= "true" > Беговая дорожка</option>
  <option value = "9" data-gray= "/uploads/sports_small/9_6_skiingdownhill_g.png?1346232174" data-white= "/uploads/sports_small/9_6_skiingdownhill_w_white.png?1346232175" data-distance= "true" > Лыжи горные</option>
  <option value = "66" data-gray= "/uploads/sports_small/66_55_20_rollerskates_g_thumbnail.png?1369737017" data-white= "/uploads/sports_small/66_55_20_rollerskates_w_white_thumbnail_white.png?1369737019" data-distance= "true" > Лыжероллеры </ option >
  < option value= "7" data-gray= "/uploads/sports_small/7_20_rollerskates_g.png?1354644506" data-white= "/uploads/sports_small/7_20_rollerskates_w_white.png?1354644507" data-distance= "true" > Ролики </ option >
  < option value= "55" data-gray= "/uploads/sports_small/55_20_rollerskates_g.png?1354644536" data-white= "/uploads/sports_small/55_20_rollerskates_w_white.png?1354644536" data-distance= "true" > Ролики спорт</option>
  <option value = "58" data-gray= "/uploads/sports_small/58_large.png?1352997781" data-white= "/uploads/sports_small/58_large_(1)_white.png?1352997781" data-distance= "true" > Скандинавская ходьба</option>
  <option value = "10" data-gray= "/uploads/sports_small/10_large_(6).png?1352893579" data-white= "/uploads/sports_small/10_large_(8)_white.png?1352893579" data-distance= "true" > Сноуборд </ option >
  < option value= "16" data-gray= "/uploads/sports_small/16_39_black_tr.png?1391098200" data-white= "/uploads/sports_small/16_39_tr_white.png?1391098200" data-distance= "true" > Спортивная ходьба</option>
  <option value = "18" data-gray= "/uploads/sports_small/18_29_orienteering_g.png?1359721779" data-white= "/uploads/sports_small/18_29_orienteering_w_white.png?1359721780" data-distance= "true" > Спортивное ориентирование</option>
  <option value = "76" data-gray= "/uploads/sports_small/76_48_black_(2).png?1392044207" data-white= "/uploads/sports_small/76_48_tr_(1)_white.png?1392044207" data-distance= "false" > TRX </ option >
  < option value= "38" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Другое </ option >
  < option value= "61" data-gray= "/uploads/sports_small/61_21_7_swimming_g_thumbnail.png?1353867894" data-white= "/uploads/sports_small/61_21_7_swimming_w_white_thumbnail_white.png?1353867894" data-distance= "false" > Аквааэробика </ option >
  < option value= "79" data-gray= "/uploads/sports_small/79_54_black.png?1392114783" data-white= "/uploads/sports_small/79_54_tr_(1)_white.png?1392114783" data-distance= "false" > Акробатика </ option >
  < option value= "23" data-gray= "/uploads/sports_small/23_26_aerobics_g.png?1355335380" data-white= "/uploads/sports_small/23_26_aerobics_w_white.png?1355335380" data-distance= "false" > Аэробика </ option >
  < option value= "26" data-gray= "/uploads/sports_small/26_large_(10).png?1352982991" data-white= "/uploads/sports_small/26_large_(11)_white.png?1352982991" data-distance= "false" > Бокс </ option >
  < option value= "84" data-gray= "/uploads/sports_small/84_56_3_mountbiking_g.png?1393840635" data-white= "/uploads/sports_small/84_56_3_mountbiking_w_white_white.png?1393840635" data-distance= "true" > Велокросс </ option >
  < option value= "24" data-gray= "/uploads/sports_small/24_30_badminton_g.png?1364125163" data-white= "/uploads/sports_small/24_30_badminton_w_white.png?1364125164" data-distance= "false" > Бадминтон </ option >
  < option value= "52" data-gray= "/uploads/sports_small/52_25_volleyball_g.png?1355335407" data-white= "/uploads/sports_small/52_25_volleyball_w_white.png?1355335407" data-distance= "false" > Волейбол </ option >
  < option value= "50" data-gray= "/uploads/sports_small/50_large_(20).png?1353949396" data-white= "/uploads/sports_small/50_large_(23)_white.png?1353949396" data-distance= "false" > Единоборства </ option >
  < option value= "49" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Гандбол </ option >
  < option value= "48" data-gray= "/uploads/sports_small/48_32_gymnastics_g.png?1364125309" data-white= "/uploads/sports_small/48_32_gymnastics_w_white.png?1364125309" data-distance= "false" > Гимнастика </ option >
  < option value= "4" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Гольф </ option >
  < option value= "13" data-gray= "/uploads/sports_small/13_23_rowing_g.png?1355335445" data-white= "/uploads/sports_small/13_23_rowing_w_white.png?1355335446" data-distance= "true" > Гребля </ option >
  < option value= "36" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Дайвинг </ option >
  < option value= "85" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Двоеборье </ option >
  < option value= "69" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Дельтаплан </ option >
  < option value= "47" data-gray= "/uploads/sports_small/47_large_(30).png?1354273685" data-white= "/uploads/sports_small/47_large_(32)_white.png?1354273685" data-distance= "false" > Йога </ option >
  < option value= "45" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Кайтсёрфинг </ option >
  < option value= "80" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Керлинг </ option >
  < option value= "62" data-gray= "/uploads/sports_small/62_34_horse_g.png?1364125353" data-white= "/uploads/sports_small/62_34_horse_w_white.png?1364125353" data-distance= "true" > Конный спорт</option>
  <option value = "46" data-gray= "/uploads/sports_small/46_21_skates_g.png?1354644602" data-white= "/uploads/sports_small/46_21_skates_w_white.png?1354644603" data-distance= "true" > Коньки </ option >
  < option value= "71" data-gray= "/uploads/sports_small/71_54_large_(5)_thumbnail.png?1381941154" data-white= "/uploads/sports_small/71_54_large_(7)_white_thumbnail_white.png?1381941154" data-distance= "false" > Кроссфит </ option >
  < option value= "64" data-gray= "/uploads/sports_small/64_50_black.png?1392042924" data-white= "/uploads/sports_small/64_50_tr_white.png?1392042924" data-distance= "false" > Круговая тренировка</option>
  <option value = "78" data-gray= "/uploads/sports_small/78_47_black.png?1392043611" data-white= "/uploads/sports_small/78_47_tr_(1)_white.png?1392043611" data-distance= "true" > Мотоспорт </ option >
  < option value= "44" data-gray= "/uploads/sports_small/44_22_mma_g.png?1354644560" data-white= "/uploads/sports_small/44_22_mma_w_white.png?1354644577" data-distance= "false" > ММА </ option >
  < option value= "70" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Параплан </ option >
  < option value= "35" data-gray= "/uploads/sports_small/35_large_(21).png?1353949363" data-white= "/uploads/sports_small/35_large_(24)_white.png?1353949364" data-distance= "false" > Пилатес </ option >
  < option value= "20" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Поло </ option >
  < option value= "73" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Растяжка </ option >
  < option value= "33" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Регби </ option >
  < option value= "60" data-gray= "/uploads/sports_small/60_41_black.png?1392052967" data-white= "/uploads/sports_small/60_41_tr_(1)_white.png?1392052967" data-distance= "true" > Рыбалка </ option >
  < option value= "67" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Самокат </ option >
  < option value= "15" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Виндсерфинг </ option >
  < option value= "63" data-gray= "/uploads/sports_small/63_35_rockclimbing_g.png?1364125396" data-white= "/uploads/sports_small/63_35_rockclimbing_w_white.png?1364125396" data-distance= "true" > Скалолазание </ option >
  < option value= "42" data-gray= "/uploads/sports_small/42_40_black_tr.png?1391098494" data-white= "/uploads/sports_small/42_40_tr_white.png?1391098494" data-distance= "false" > Сквош </ option >
  < option value= "41" data-gray= "/uploads/sports_small/41_38_black_tr.png?1391098095" data-white= "/uploads/sports_small/41_38_tr_(1)_white.png?1391098095" data-distance= "true" > Скейтборд </ option >
  < option value= "68" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "true" > Спорт </ option >
  < option value= "75" data-gray= "/uploads/sports_small/75_43_black.png?1391097588" data-white= "/uploads/sports_small/75_43_tr_white.png?1391097588" data-distance= "false" > Степпер </ option >
  < option value= "29" data-gray= "/uploads/sports_small/29_large_(22).png?1353949425" data-white= "/uploads/sports_small/29_large_(25)_white.png?1353949425" data-distance= "false" > Танцы </ option >
  < option value= "40" data-gray= "/uploads/sports_small/40_27_tenniss_g.png?1356615508" data-white= "/uploads/sports_small/40_27_tenniss_w_white.png?1356615508" data-distance= "false" > Теннис </ option >
  < option value= "37" data-gray= "/uploads/sports_small/37_33_pingpong_g.png?1364125438" data-white= "/uploads/sports_small/37_33_pingpong_w_white.png?1364125439" data-distance= "false" > Теннис настольный</option>
  <option value = "43" data-gray= "/uploads/sports_small/43_24_hiking_g.png?1355335482" data-white= "/uploads/sports_small/43_24_hiking_w_white.png?1355335482" data-distance= "true" > Туризм </ option >
  < option value= "81" data-gray= "/uploads/sports_small/81_71_54_large_(5)_thumbnail_thumbnail.png?1387304830" data-white= "/uploads/sports_small/81_71_54_large_(7)_white_thumbnail_white_thumbnail_white.png?1387304830" data-distance= "false" > Уличный тренажёр</option>
  <option value = "31" data-gray= "/uploads/sports_small/31_19_soccer_g.png?1363965892" data-white= "/uploads/sports_small/31_19_soccer_w_white.png?1363965892" data-distance= "false" > Футбол </ option >
  < option value= "59" data-gray= "/uploads/sports_small/59_28_fencing_g.png?1358700192" data-white= "/uploads/sports_small/59_28_fencing_w_white.png?1358700193" data-distance= "false" > Фехтование </ option >
  < option value= "39" data-gray= "/assets/default/sport_small.png" data-white= "/assets/default/sport_white_small.png" data-distance= "false" > Фигурное катание</option>
  <option value = "34" data-gray= "/uploads/sports_small/34_large_(12).png?1353666695" data-white= "/uploads/sports_small/34_large_(13)_white.png?1353666695" data-distance= "false" > Хоккей </ option >
  < option value= "82" data-gray= "/uploads/sports_small/82_53_black.png?1400754761" data-white= "/uploads/sports_small/82_53_tr_(1)_white.png?1400754761" data-distance= "false" > Шахматы </ option >
  < option value= "74" data-gray= "/uploads/sports_small/74_42_black.png?1391097719" data-white= "/uploads/sports_small/74_42_tr_(1)_white.png?1391097719" data-distance= "true" > Эллипс </ option ></ select >
                                                                                                                                                                                                          */