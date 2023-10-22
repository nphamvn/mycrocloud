export default function debounce<T extends (...args: any[]) => void>(fn: T, delay: number) {
    let timerId: number | undefined = undefined;
    return function (this: any, ...args: any[]) {
        clearTimeout(timerId)
        timerId = setTimeout(() => { fn.apply(this, args) }, delay);
    }
}