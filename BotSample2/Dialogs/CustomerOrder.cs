using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.FormFlow;



namespace BotSample2.Dialogs
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "Can't understand \"{0}\", please input again.")]
    [Template(TemplateUsage.EnumSelectOne,"Which {&} ？{||}",ChoiceStyle=ChoiceStyleOptions.PerLine)]
    public class SandwichOrder
    {
        [Prompt("Choose yoru {&}? {||}")]
        [Describe("Sandwich")]
        public SandwichOptions? Sandwich;

        public LengthOptions? Length;
        public BreadOptions? Bread;
        [Optional]
        public CheeseOptions? Cheese;
        public List<ToppingOptions> Toppings;
        public List<SauceOptions> Sauce;
        public static IForm<SandwichOrder> BuildForm()
        {
            return new FormBuilder<SandwichOrder>()
                    .Message("Welcome to our Sandwich maker Bot")
                    .Build();
        }
    }


    #region enum

    public enum SandwichOptions
    {
        [Describe("Bacon Lattace Tomato")]
        BLT,
        BlackForestHam,
        BuffaloChicken,
        ChickenAndBaconRanchMelt,
        ColdCutCombo,
        MeatballMarinara,
        OvenRoastedChicken,
        RoastBeef,
        RotisserieStyleChicken,
        SpicyItalian,
        SteakAndCheese,
        SweetOnionTeriyaki,
        Tuna,
        TurkeyBreast,
        Veggie
    }



    public enum LengthOptions { SixInch, FootLong }
    public enum BreadOptions { NineGrainWheat, NineGrainHoneyOat, Italian, ItalianHerbsAndCheese, Flatbread }
    public enum CheeseOptions { American, MontereyCheddar, Pepperjack }
    public enum ToppingOptions
    {
        Avocado,
        BananaPeppers,
        Cucumbers,
        GreenBellPeppers,
        Jalapenos,
        Lettuce,
        Olives,
        Pickles,
        RedOnion,
        Spinach,
        Tomatoes
    }



    public enum SauceOptions
    {
        ChipotleSouthwest,
        HoneyMustard,
        LightMayonnaise,
        RegularMayonnaise,
        Mustard,
        Oil,
        Pepper,
        Ranch,
        SweetOnion,
        Vinegar
    }

    #endregion

}