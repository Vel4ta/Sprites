import { useState, useRef, useEffect } from "react";
import Sprite from "./Sprite";
import animation from "./Animations";
import { getMultipleRequests, min } from './Utils';
export default function Table({world_map, position, spritesheets, display_amount, texture, setSelection }) {
    const [info, setInfo] = useState(handleClick(0, {
        index: 0,
        flip: false,
        direction: false,
        incrimentX: texture.incrimentX
    }, spritesheets, display_amount));
    const map_data_collected = useRef(false);
    useEffect(() => {
        if (map_data_collected.current) { return; }
        if (world_map.current == null) { world_map.current = []; }
        world_map.current = world_map.current.concat([texture.incrimentY, info.incrimentX * display_amount, position.x, position.y]);
        map_data_collected.current = !map_data_collected.current;
    }, [display_amount, texture, position, world_map, info]);
    return (<div style={
        {
            position: "absolute",
            left: position.x + "px",
            bottom: position.y + "px"
        }}>
        <button onClick={() => setInfo(handleClick(-1, info, spritesheets, display_amount))}>left</button>
        <div style={{
            backgroundImage: `url(${texture.src})`,
            height: texture.incrimentY + "px",
            width: (info.incrimentX * display_amount) + "px",
            backgroundRepeat: "repeat-x",
            backgroundColor: "green",
            position: "relative",
            overflow: "clip visible",
        }}>
            {spritesheets.map((sprite, index) =>
                <Slide key={sprite.info.id}
                    style={
                        {
                            position: "absolute",
                            bottom: texture.incrimentY / 2 - sprite.info.incrimentY / 2 + "px"
                        }}
                    slide={
                        {
                            i1: -(info.index + (!info.direction ? 1 : -1) - index) * info.incrimentX,
                            i2: (info.direction ? 1 : -1) * (info.incrimentX + (info.direction ? -1 : 1) * (info.incrimentX / 2 - sprite.info.incrimentX / 2)),
                            flip: info.flip,
                            direction: info.direction,
                            duration: 100 * display_amount
                        }
                    }
                    setSelection={setSelection}
                    world_map={world_map}
                    position={position}
                    {...sprite.info}
                ></Slide>
            )}
        </div>
        <button onClick={() => setInfo(handleClick(1, info, spritesheets, display_amount))}>right</button>
    </div>);
}

function handleClick(dir, info, spritesheets, display_amount) {
    var count = 0;
    var cur = min((info.index + dir) * display_amount, info.index + dir * display_amount);
    var incX = info.incrimentX;
    const flip = (dir > 0 && !info.direction) || (dir < 0 && info.direction);
    const space_available = info.incrimentX * display_amount;
    while (cur >= 0 && cur < spritesheets.length) {
        count += spritesheets[cur++].info.incrimentX;
        if (count <= space_available) {
            continue;
        }

        if (spritesheets[cur - 1].info.incrimentX > incX && space_available / count < 1) {
            incX = spritesheets[cur - 1].info.incrimentX;
            break;
        }
    }
    return count > 0 ? {
        index: info.index + dir * display_amount,
        direction: dir > 0,
        flip: flip,
        incrimentX: incX
    } : info;
}

function Slide({ slide, style, setSelection, world_map, position, ...props }) {
    const slideRef = useRef(null);
    useEffect(() => {
        const a = animation({
            callback: (p) => {
                if (slideRef.current) {
                    slideRef.current.style.left = slide.i1 - slide.i2 * Math.sin(Math.PI / 2 * p) + "px";
                }
            },
            duration: slide.duration
        });
        const id = requestAnimationFrame(a);
        return () => cancelAnimationFrame(id);
    }, [slide]);
    return <div ref={slideRef} style={style}
        onClick={async () =>
            await getMultipleRequests([
                {
                    operation: "GetAction",
                    parameters: [props.title]
                },
                {
                    operation: "GetBounds",
                    parameters: [
                        window.innerHeight,
                        window.innerWidth,
                        props.incrimentX,
                        props.incrimentY,
                    ].concat(world_map.current)
                }
            ]).then(([selection, bounds]) => setSelection(selection, bounds, position))
        }>
        <Sprite {...props}></Sprite>
    </div>;
}

function SpriteInfo({ visible, ...props }) {
    return visible && <div>
        <p>some info here</p>
    </div>;
}