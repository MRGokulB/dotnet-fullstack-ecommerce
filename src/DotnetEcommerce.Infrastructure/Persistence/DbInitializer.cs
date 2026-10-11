using DotnetEcommerce.Domain.Entities;
using DotnetEcommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DotnetEcommerce.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Products.AnyAsync())
        {
            return;
        }

        var catHardware = Category.Create(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Developer Hardware",
            "hardware",
            "Microcontrollers, macropads, and hardware hacking toolkits",
            "Cpu"
        );

        var catAudio = Category.Create(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Studio Audio",
            "audio",
            "High-fidelity monitors, planar headphones, and broadcast microphones",
            "Headphones"
        );

        var catWorkspace = Category.Create(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "Ergonomic Workspace",
            "workspace",
            "Precision desk risers, leather desk pads, and cable routing trays",
            "Layers"
        );

        var catKeyboards = Category.Create(
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            "Mechanical Keyboards",
            "keyboards",
            "Custom CNC aluminum cases, tactile switches, and PBT keycaps",
            "Keyboard"
        );

        await context.Categories.AddRangeAsync(catHardware, catAudio, catWorkspace, catKeyboards);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            Product.Create(
                Guid.NewGuid(),
                "Ergonomic Split Ortholinear Keyboard",
                "split-ortholinear-keyboard",
                "Anodized CNC aluminum casing with hot-swappable Lubed Boba U4T switches and OLED layer screens.",
                279.00m,
                "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=800&q=80",
                catKeyboards.Id,
                18,
                4.9,
                94,
                true
            ),
            Product.Create(
                Guid.NewGuid(),
                "Planar Magnetic Studio Reference Headphones",
                "planar-studio-headphones",
                "Open-back planar transducer technology delivering ruler-flat acoustic frequency response for audio engineers.",
                429.00m,
                "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=800&q=80",
                catAudio.Id,
                12,
                4.8,
                68,
                true
            ),
            Product.Create(
                Guid.NewGuid(),
                "Solid Walnut Dual Monitor Riser",
                "walnut-dual-monitor-riser",
                "Handcrafted kiln-dried American walnut with integrated anodized aluminum trays and cable pass-through channels.",
                169.00m,
                "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?auto=format&fit=crop&w=800&q=80",
                catWorkspace.Id,
                24,
                4.9,
                120,
                true
            ),
            Product.Create(
                Guid.NewGuid(),
                "Precision Anodized Macropad 16K",
                "precision-macropad-16k",
                "16 programmable mechanical keys with clickable encoder knob for Docker, Git, and timeline workflows.",
                129.00m,
                "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?auto=format&fit=crop&w=800&q=80",
                catHardware.Id,
                35,
                4.7,
                41,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "Studio Dynamic XLR / USB-C Microphone",
                "studio-dynamic-microphone",
                "Broadcast-quality cardioid dynamic capsule with built-in 24-bit/96kHz DAC and real-time DSP pop filter.",
                199.00m,
                "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?auto=format&fit=crop&w=800&q=80",
                catAudio.Id,
                22,
                4.8,
                83,
                true
            ),
            Product.Create(
                Guid.NewGuid(),
                "Wireless 3-in-1 Fast MagSafe Station",
                "magsafe-3in1-station",
                "Machined space-gray aluminum dock delivering fast 15W Qi2 wireless charging to phone, watch, and earbuds.",
                99.00m,
                "https://images.unsplash.com/photo-1586953208448-b95a79798f07?auto=format&fit=crop&w=800&q=80",
                catWorkspace.Id,
                45,
                4.6,
                57,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "Custom 65% Gasket-Mount Mechanical Keyboard",
                "gasket-mount-65-keyboard",
                "Poron foam isolation gaskets, FR4 switch plate, per-key RGB backlighting, and south-facing switch sockets.",
                219.00m,
                "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&w=800&q=80",
                catKeyboards.Id,
                16,
                4.9,
                72,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "USB-C Hardware Protocol Analyzer & Flipper",
                "hardware-protocol-analyzer",
                "High-speed logic analyzer, sub-GHz transceiver, and hardware debugging bus probe for embedded systems.",
                189.00m,
                "https://images.unsplash.com/photo-1518770660439-4636190af475?auto=format&fit=crop&w=800&q=80",
                catHardware.Id,
                14,
                4.9,
                38,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "Full-Grain Merino Wool & Leather Desk Mat",
                "wool-leather-desk-mat",
                "Water-resistant dense felt backing with genuine vegetable-tanned leather resting strip (900mm x 400mm).",
                79.00m,
                "https://images.unsplash.com/photo-1544816155-12df9643f363?auto=format&fit=crop&w=800&q=80",
                catWorkspace.Id,
                55,
                4.7,
                92,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "Active Nearfield Bi-Amplified Studio Monitors",
                "bi-amplified-studio-monitors",
                "Class-D bi-amped 5-inch Kevlar woofers with custom waveguide silk tweeters for acoustic transparency.",
                349.00m,
                "https://images.unsplash.com/photo-1545454675-3531b543be5d?auto=format&fit=crop&w=800&q=80",
                catAudio.Id,
                8,
                4.8,
                51,
                true
            ),
            Product.Create(
                Guid.NewGuid(),
                "Double-Shot PBT Cherry Profile Keycap Set",
                "doubleshot-pbt-keycaps",
                "Thick 1.5mm PBT keycaps with crisp legends and MX stem compatibility in stealth dark slate colorway.",
                69.00m,
                "https://images.unsplash.com/photo-1618384887929-16ec33fab9ef?auto=format&fit=crop&w=800&q=80",
                catKeyboards.Id,
                40,
                4.8,
                63,
                false
            ),
            Product.Create(
                Guid.NewGuid(),
                "Hardware Cryptographic Security Key FIDO2",
                "fido2-hardware-security-key",
                "Dual USB-C and NFC hardware authenticator supporting FIDO2, WebAuthn, and PIV smart card capabilities.",
                59.00m,
                "https://images.unsplash.com/photo-1563770660941-20978e870e26?auto=format&fit=crop&w=800&q=80",
                catHardware.Id,
                60,
                4.9,
                135,
                false
            )
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var demoOrder1 = Order.Create(
            "alex.chen@google.com",
            "Alex Chen",
            "742 Evergreen Terrace, Mountain View, CA 94043",
            448.00m,
            35.84m,
            0.00m
        );
        demoOrder1.AddItem(products[0].Id, products[0].Name, products[0].Price, 1);
        demoOrder1.AddItem(products[2].Id, products[2].Name, products[2].Price, 1);
        demoOrder1.AttachStripeSession("cs_live_sample_session_9921");
        demoOrder1.MarkAsPaid("pi_3MtwxRLkdIwHu7ix28a3tKZX");

        var demoOrder2 = Order.Create(
            "sarah.connor@netflix.com",
            "Sarah Connor",
            "100 Winchester Blvd, Los Gatos, CA 95032",
            429.00m,
            34.32m,
            0.00m
        );
        demoOrder2.AddItem(products[1].Id, products[1].Name, products[1].Price, 1);
        demoOrder2.AttachStripeSession("cs_live_sample_session_4188");
        demoOrder2.MarkAsPaid("pi_3Mtwy2LkdIwHu7ix19b4uABZ");

        var demoOrder3 = Order.Create(
            "david.miller@stripe.com",
            "David Miller",
            "510 Townsend St, San Francisco, CA 94103",
            129.00m,
            10.32m,
            15.00m
        );
        demoOrder3.AddItem(products[3].Id, products[3].Name, products[3].Price, 1);
        demoOrder3.AttachStripeSession("cs_pending_sample_session_1042");

        await context.Orders.AddRangeAsync(demoOrder1, demoOrder2, demoOrder3);
        await context.SaveChangesAsync();
    }
}
