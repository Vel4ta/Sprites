import { useRef, useEffect } from 'react';
import animation from "./Animations";
import Table from "./Table";
import { getBounds } from "./Utils";
import Player from './Player';

export default function Map({ player, world_map }) {
    const bounds = useRef(null);
    console.log(bounds.map);
    useEffect(() => {
        if (bounds.current) { return; }
        getBounds(player.incrimentX, player.incrimentY, window.innerWidth, window.innerHeight, world_map, bounds);
    }, [player, world_map]);
    return <Player {...player} bounds={bounds }></Player>;
}