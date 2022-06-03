using System;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;

namespace YukiChan.Utils;

public static class CaptchaUtil
{
    public static void OnCaptcha(Bot bot, CaptchaEvent e)
    {
        switch (e.Type)
        {
            case CaptchaEvent.CaptchaType.Sms:
                Console.WriteLine($"The verify code has been sent to {e.Phone}.");
                Console.Write("Code: ");
                bot.SubmitSmsCode(Console.ReadLine());
                break;

            case CaptchaEvent.CaptchaType.Slider:
                Console.WriteLine("Need slider captcha.");
                Console.Write($"Url: {e.SliderUrl}\nTicket: ");
                bot.SubmitSliderTicket(Console.ReadLine());
                break;

            case CaptchaEvent.CaptchaType.Unknown:
            default:
                break;
        }
    }
}