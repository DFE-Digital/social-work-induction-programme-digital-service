/* This ESM replaces profile links on mod data pages with plain text in a span */
const convertUserLinks = () => {
    document.querySelectorAll('div > a[href*="/user/view.php"]').forEach(a => {
        const span = document.createElement('span');
        span.className = a.className || '';
        span.textContent = a.textContent;
        a.replaceWith(span);
    });
};

export const init = () => {
    if (!document.body.classList.contains('path-mod-data')) {
        return;
    }
    convertUserLinks();
    document.addEventListener('core-templates-rendered', convertUserLinks);
};