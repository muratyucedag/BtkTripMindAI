export function createUserMessage(message) {
    const userMessageHtml = `
                <div class="flex gap-4 items-start justify-end self-end max-w-[85%]">
                    <div class="bg-primary-container/40 p-5 rounded-2xl rounded-tr-none border border-primary/10">
                        <p class="text-body-md text-on-primary-container"></p>
                    </div>

                    <div class="w-10 h-10 rounded-full bg-secondary-fixed flex-shrink-0 overflow-hidden">
                        <img class="w-full h-full object-cover"
                             alt="Kullanýcý profil fotoðrafý"
                             src="https://lh3.googleusercontent.com/aida-public/AB6AXuCJ9m_0uCYjopEIK8G1vB_YYt2Unk0rfggH8Z5aCNyHXvyjzmI-XpQRnUUzbBnCyAD_dh1IsIxjA73JSkT9UG44QbDzwMko5A04jLsMefCKVId_ITvvlJS-eTmZNJvG-wkXqO0rAVF3VPpMOV22X2YHpwMK1nE_NFkEIaspYOpRwG1OQLQrNRUHp_7e4-O1bqsIxwTGa5Yj8L6Mape0_bhu9j4gBGOXaj3FmScFwbCC_g5WFJYoHuHgPA" />
                    </div>
                </div>
            `;

    const wrapper = document.createElement("div");
    wrapper.innerHTML = userMessageHtml.trim();

    const messageElement = wrapper.firstElementChild;
    const messageText = messageElement.querySelector("p");

    messageText.textContent = message;

    chatMessages.insertBefore(messageElement, aiThinking);
}

export function createAgentMessage(message) {
    const agentMessageHtml = `
                <div class="flex gap-4 items-start max-w-[85%]">
                    <div class="w-10 h-10 rounded-full bg-primary flex-shrink-0 flex items-center justify-center text-white">
                        <span class="material-symbols-outlined">
                            smart_toy
                        </span>
                    </div>

                    <div class="bg-white p-5 rounded-2xl rounded-tl-none shadow-sm border border-outline-variant/20">
                        <p class="text-body-md text-on-surface whitespace-pre-line"></p>
                    </div>
                </div>
            `;

    const wrapper = document.createElement("div");
    wrapper.innerHTML = agentMessageHtml.trim();

    const messageElement = wrapper.firstElementChild;
    const messageText = messageElement.querySelector("p");

    messageText.textContent = message;

    chatMessages.insertBefore(messageElement, aiThinking);
}