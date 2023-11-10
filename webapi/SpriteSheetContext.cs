using Microsoft.EntityFrameworkCore;

namespace webapi;

public class SpriteSheetContext : DbContext
{
    public SpriteSheetContext(DbContextOptions<SpriteSheetContext> options)
    : base(options)
    {
    }

    public DbSet<Sprite> sprites { get; set; } = null!;

    public DbSet<SpriteSettings> sprite_settings { get; set; } = null!;

    public IEnumerable<SpriteSheet> spriteSheetData => sprites.Join(sprite_settings,
        sprite => sprite.Id,
        settings => settings.Id,
        (sprite, settings) => new SpriteSheet
        {
            Id = "" + sprite.Id + "" + settings.Settings_Id,
            Title = sprite.Title,
            Src = sprite.Src,
            Width = sprite.Width,
            Height = sprite.Height,
            Frames = sprite.Frames,
            StartFrame = settings.StartFrame,
            Rows = sprite.Rows,
            Row = settings.Row,
            Duration = sprite.Duration,
            Event = settings.Event
        }).ToList();
}
