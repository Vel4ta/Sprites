export default function animation({ callback, duration }) {
    var start, allowed = false;
    function loop(timestamp) {
        if (!allowed) {
            start = timestamp;
        }

        const elapsed = timestamp - start, p = elapsed / duration;
        allowed = elapsed <= duration;
        if (allowed) {
            callback(p);
            requestAnimationFrame(loop);
        } else {
            requestAnimationFrame(() => callback(1));
        }
    }
    return loop;
}