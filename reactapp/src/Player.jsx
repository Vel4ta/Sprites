import { useRef, useEffect } from 'react';
import animation from "./Animations";

export default function Player({ frames, incrimentX, incrimentY, duration, actions, src, bounds, position }) {
    const elementRef = useRef(null);
    const flip = useRef(1);
    const timeout_id = useRef(0);
    const acting = useRef(false);
    console.log(bounds);
    useEffect(() => {
        if (!bounds.current) { return; }
        function handler(e) {
            if (elementRef.current) {
                var xhelp = 0;
                var yhelp = 0;
                const prev = flip.current;
                switch (e.key) {
                    case "w":
                        yhelp += incrimentY;
                        break;
                    case "s":
                        yhelp -= incrimentY;
                        break;
                    case "a":
                        xhelp -= incrimentX;
                        flip.current = -1;
                        break;
                    case "d":
                        xhelp += incrimentX;
                        flip.current = 1;
                        break;
                    default: return;
                }

                if (prev != flip.current) { elementRef.current.style.transform = "ScaleX(" + flip.current + ")"; }

                const offsetX = actions[e.key].offsetX;
                const offsetY = actions[e.key].offsetY;
                const x = (((position.current.x + xhelp) / incrimentX) + .5) | 0;
                const y = (((position.current.y + yhelp) / incrimentY) + .5) | 0;

                if (y >= 0 && y < bounds.current.h && x >= 0 && x < bounds.current.w && !bounds.current.map[y][x]) {
                    const a = animation({
                        callback: (p) => {
                            elementRef.current.style.backgroundPosition = offsetX + Math.ceil(frames * p) * incrimentX + "px " + offsetY + "px";

                            elementRef.current.style.bottom = position.current.y + yhelp * p + "px";

                            elementRef.current.style.left = position.current.x + xhelp * p + "px";

                            if (p == 1) {
                                position.current.x += xhelp;
                                position.current.y += yhelp;
                            }
                        },
                        duration: duration
                    });
                    requestAnimationFrame(a);
                } else {
                    elementRef.current.style.backgroundPosition = offsetX * incrimentX + "px " + offsetY + "px";
                }
            }
        }
        function handler_wrap(e) {
            if (acting.current) { return; }
            acting.current = !acting.current;
            clearTimeout(timeout_id.current);
            timeout_id.current = setTimeout(() => {
                handler(e);
                acting.current = !acting.current;
            }, duration + 32);
        }
        window.addEventListener("keydown", handler_wrap, false);
        return () => window.removeEventListener("keydown", handler_wrap, false);
    }, [incrimentX, incrimentY, frames, duration, bounds, actions, position]);
    const props = {
        ref: elementRef,
        style: {
            backgroundImage: `url(${src}) `, backgroundPosition: actions.d.offsetX + "px " + actions.d.offsetY + "px", height: incrimentY + "px", width: incrimentX + "px",
            backgroundColor: "red", padding: 0, position: "absolute", left: position.current.x + "px", bottom: position.current.y + "px"
        }
    };
    return <div {...props}></div>;
}