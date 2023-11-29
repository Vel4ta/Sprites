using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Web;
using static Azure.Core.HttpHeader;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class SpriteSheetController : ControllerBase
{
    private readonly ILogger<SpriteSheetController> _logger;

    private readonly SpriteSheetContext _context;

    //private readonly IEnumerable<SpriteSheet> _context;
    public SpriteSheetController(SpriteSheetContext context, ILogger<SpriteSheetController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet(Name = "GetSpriteSheet")]
    public IEnumerable<SpriteSheet> Get()
    {
        return _context.spriteSheetData;
    }

    [HttpPost(Name = "GetSpriteSheet"), Route("[action]")]
    public IEnumerable<SpriteSheet> Get(List<SpriteIdentifier> idents)
    {
        List<SpriteSheet> result = new List<SpriteSheet>();
        foreach (SpriteIdentifier ident in idents)
        {
            var item = _context.spriteSheetData.Where(p => p.Id == ident.ToString()).FirstOrDefault();
            if (item != null)
            {
                result.Add(item);
            }
        }
        return result;
    }

    [HttpPost(Name = "PostSpriteSheet"), Route("[action]")]
    public ActionResult GetAction(Sprite ident)
    {
        return Ok(makeAction(ident));
    }

    public object? makeAction( Sprite ident )
    {
        var items = _context.spriteSheetData.Where(p => p.Title == ident.Title);
        if (items != null)
        {
            SpriteSheet item = items.First();
            var action_w = items.Where(p => p.Event == "w").FirstOrDefault();
            var action_s = items.Where(p => p.Event == "s").FirstOrDefault();
            var action_d = items.Where(p => p.Event == "d").FirstOrDefault();
            var action_a = items.Where(p => p.Event == "a").FirstOrDefault();

            double incrimentX = item.Frames > 0 ? item.Width / item.Frames : 0;
            double incrimentY = item.Rows > 0 ? item.Height / item.Rows : 0;
            return new
            {
                item.Title,
                item.Src,
                item.Width,
                item.Height,
                item.Frames,
                item.Rows,
                item.Duration,
                incrimentX,
                incrimentY,
                Actions = new
                {
                    w = new
                    {
                        offsetX = action_w != null ? action_w.StartFrame * incrimentX : 0,
                        offsetY = action_w != null ? action_w.Row * incrimentY : 0,
                    },
                    s = new
                    {
                        offsetX = action_s != null ? action_s.StartFrame * incrimentX : 0,
                        offsetY = action_s != null ? action_s.Row * incrimentY : 0,
                    },
                    d = new
                    {
                        offsetX = action_d != null ? action_d.StartFrame * incrimentX : 0,
                        offsetY = action_d != null ? action_d.Row * incrimentY : 0,
                    },
                    a = new
                    {
                        offsetX = action_a != null ? action_a.StartFrame * incrimentX : 0,
                        offsetY = action_a != null ? action_a.Row * incrimentY : 0,
                    },
                }
            }; ;
        }

        return null;
    }

    [HttpPost(Name = "PostBounds"), Route("[action]")]
    public ActionResult GetBounds(Bounds bounds)
    {
        return Ok(bounds.makeBounds());
    }

    [HttpPost(Name = "PostMultipleRequests"), Route("[action]")]
    public ActionResult MultipleRequests(List<Request> requests)
    {
        List<object> responses = new List<object>();
        foreach (Request request in requests)
        {
            switch (request.Operation)
            {
                case "GetBounds":
                    responses.Add(new Bounds(request.Args).makeAABBTree());
                    break;
                case "GetAction":
                    responses.Add(makeAction(new Sprite { Title = request.Args[0]}));
                    break;
                default: break;
            }
        }
        return Ok(responses);
    }
}
