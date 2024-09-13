using System.ComponentModel.DataAnnotations;

namespace ExchangeData
{
    public enum RabbitRoutingKeys
    {
        [Display(Name = "Referal")]
        Referal,
        [Display(Name = "Craft")]
        Craft,
        [Display(Name = "FinishHunt")]
        FinishHunt
    }
}
