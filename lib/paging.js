export const PAGE_SIZE = 12;

export function paginate(items, root) {
  const total = Math.max(1, Math.ceil(items.length / PAGE_SIZE));
  const href = (n) => (n === 1 ? root : `${root}page/${n}/`);
  return Array.from({ length: total }, (_, i) => {
    const number = i + 1;
    return {
      number,
      total,
      items: items.slice(i * PAGE_SIZE, number * PAGE_SIZE),
      href: href(number),
      first: number > 1 ? href(1) : null,
      previous: number > 1 ? href(number - 1) : null,
      next: number < total ? href(number + 1) : null,
      last: number < total ? href(total) : null,
    };
  });
}
