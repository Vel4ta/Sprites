import { useRef } from 'react';
import animation from "./Animations";

export default function Sprite({ frames, incrimentX, incrimentY, duration, offsetX, offsetY, src }) {
    const elementRef = useRef(null);
    const props = {
        onMouseOver: () => {
            const a = animation({
                callback: (p) => {
                    if (elementRef.current) {
                        const x = offsetX + (((frames * p) + .5) | 0) * incrimentX;
                        elementRef.current.style.backgroundPosition = "" + x + "px " + offsetY + "px";
                    }
                },
                duration: duration
            });
            requestAnimationFrame(a);
        },
        ref: elementRef,
        style: { backgroundImage: `url(${src}) `, backgroundPosition: offsetX + "px " + offsetY + "px", height: incrimentY + "px", width: incrimentX + "px", backgroundColor: "transparent", padding: 0 }
    };
    return <button {...props}></button>;
}