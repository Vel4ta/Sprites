export async function getSpriteSheetData(idents) {
    return await fetch('spritesheet/Get', {
        method: "POST",
        headers: { 'Content-type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify(idents)
    }).then(r => r.json());
}

export async function getSpriteSheetActionData(ident) {
    return await fetch('spritesheet/GetAction', {
        method: "POST",
        headers: { 'Content-type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({ title: ident })
    }).then(r => r.json());
}

export async function getBounds(incrimentX, incrimentY, width, height, world_map) {
    return await fetch('spritesheet/GetBounds', {
        method: "POST",
        headers: { 'Content-type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify({
            incrimentX: incrimentX,
            incrimentY: incrimentY,
            width: width,
            height: height,
            world_map: world_map
        })
    }).then(r => r.json());
}

export async function getMultipleRequests(requests) {
    console.log(requests);
    return await fetch('spritesheet/MultipleRequests', {
        method: "POST",
        headers: { 'Content-type': 'application/json', 'Accept': 'application/json' },
        body: JSON.stringify(requests)
    }).then(r => r.json());
}

export const min = (a, b) => a < b ? a : b;