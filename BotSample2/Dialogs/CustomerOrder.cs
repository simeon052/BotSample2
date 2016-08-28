using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;

namespace BotSample2.Dialogs
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "\"{0}\" is not allowd. Input again.")]
    [Template(TemplateUsage.EnumSelectOne, "Which {&} do you like？{||}", ChoiceStyle = ChoiceStyleOptions.PerLine)]
    public class SandwichOrder
    {
        private const string NoPreference = "No thank you.";

        #region public variables

        [Describe("Sandwich")]
        public SandwichOptions? Sandwich;

        public LengthOptions? Length;

        [Describe("Bread (Pan)")]
        public BreadOptions? Bread;

        [Optional]
        [Template(TemplateUsage.NoPreference, NoPreference)]
        public CheeseOptions? Cheese;

        [Describe("Topping")]
        [Terms("except", "but", "not", "no", "all", "everything")]
        public List<ToppingOptions> Toppings;

        public List<SauceOptions> Sauces;

        [Numeric(0, 5)]
        [Optional]
        [Describe("your experience today{||}")]
        [Template(TemplateUsage.NoPreference, NoPreference)]
        public double? Rating;

        [Optional]
        [Template(TemplateUsage.NoPreference, NoPreference)]
        [Template(TemplateUsage.EnumSelectOne, "These are free for you{||}")]
        public string Specials;

        [Optional]
        [Describe("Tell us your convinient time to deliver")]
        [Template(TemplateUsage.NoPreference, NoPreference)]
        public DateTime? DeliveryTime;

        [Describe("Address ")]
        public string DeliveryAddress;

        [Describe("Phone Number")]
        [Pattern("(\\(\\d{3}\\))?\\s*\\d{3}(-|\\s*)\\d{4}")]
        public string PhoneNumber;

        #endregion

        #region functions 

        public static IForm<SandwichOrder> BuildForm()
        {
            OnCompletionAsyncDelegate<SandwichOrder> processOrder = async (context, state) =>
            {
                await context.PostAsync("Thank you. your order was accepted.");
            };

            return new FormBuilder<SandwichOrder>()
                .Message("Welcome to our Sandwich Store")
                .Field(nameof(Sandwich))
                .Field(nameof(Length))
                .Field(nameof(Bread))
                .Field(nameof(Cheese))
                .Field(nameof(Toppings),
                        validate: async (state, value) =>
                        {
                            var values = ((List<object>)value).OfType<ToppingOptions>();
                            var result = new ValidateResult() { IsValid = true, Value = values };
                            if (values != null && values.Contains(ToppingOptions.Everything))
                            {
                                result.Value = Enum.GetValues(typeof(ToppingOptions)).Cast<ToppingOptions>()
                                                .Where(t => t != ToppingOptions.Everything && !values.Contains(t)).ToList();
                            }
                            return result;
                        })
                        .Message("{Toppings} was choosed")
                .Field(nameof(Sauces))
                .Field(new FieldReflector<SandwichOrder>(nameof(Specials))
                    .SetType(null)
                    .SetActive((state) => state.Length == LengthOptions.FootLong)
                    .SetDefine(async (stateBag, field) =>
                    {
                        field.AddDescription("cookie", "Free cookie")
                                .AddTerms("cookie", "cookie", "free cookie")
                                .AddDescription("drink", "Free large drink")
                                .AddTerms("drink", "drink", "free drink");
                        return true;
                    }))
                    .Confirm(async (state) =>
                    {
                        float cost = 0f;
                        switch (state.Length)
                        {
                            case LengthOptions.SixInch: cost = 5.00f; break;
                            case LengthOptions.FootLong: cost = 6.50f; break;
                        }
                        return new PromptAttribute($"Your total pay : {cost:C} dollars");
                    })
                .Field(nameof(DeliveryAddress),
                    validate: async (state, response) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = response };
                        var address = (response as string).Trim();
                        if (address.Length > 0 && (address[0] < '0' || address[0] > '9'))
                        {
                            result.Feedback = "Please yoru zip code first.";
                            result.IsValid = false;
                        }
                        return result;
                    })
                .Field(nameof(DeliveryTime), "Tell us time to deliver ? {||}")
                .Confirm("Confirmation : {Length} {Sandwich} {Bread} {&Bread} And {[{Cheese} {Toppings} {Sauces}]} {? Will deliver at {DeliveryTime:t}} To {DeliveryAddress}.")
                .AddRemainingFields()
                .Message("Thank you.")
                .OnCompletion(processOrder)
                .Build();
        }
        #endregion
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
        [Terms("except", "but", "not", "no", "all", "everything")]
        Everything = 1,
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