window.chatWireUpEnter = (selector, dotnetRef) => {
    const el = document.querySelector(selector);
    if (!el) return;
    el.addEventListener('keydown', (e) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            dotnetRef.invokeMethodAsync('SendFromJs');
        }
    });
};

window.scrollChatToBottom = () => {
    const wrap = document.querySelector('.chat-window');
    if (wrap) wrap.scrollTop = wrap.scrollHeight;
};
