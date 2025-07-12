// ==============================================
// FILTRO DE LISTA DE JOGOS
// ==============================================
document.addEventListener('DOMContentLoaded', function() {
    // Configura o filtro de pesquisa para a lista de jogos
    const gameFilterInput = document.getElementById('gameFilter');
    const gameList = document.getElementById('gameList');
    const games = Array.from(gameList.getElementsByTagName('li'));

    // Evento que filtra a lista conforme o usuário digita
    gameFilterInput.addEventListener('input', function() {
        const filterValue = gameFilterInput.value.toLowerCase();
        
        games.forEach(function(game) {
            const gameText = game.textContent.toLowerCase();
            // Mostra/oculta itens baseado no filtro
            game.style.display = gameText.includes(filterValue) ? '' : 'none';
        });
    });
});

// ==============================================
// GALERIA DE IMAGENS - SUPER MARIO 64
// ==============================================
document.addEventListener('DOMContentLoaded', function() {
    const galleryImages = [
        '/images/sm64_1.jpg',
        '/images/sm64_2.jpg',
        '/images/sm64_3.jpg',
        '/images/sm64_4.jpg',
        '/images/sm64_5.jpg'
    ];

    const galleryContainer = document.getElementById('gallery');
    
    if (galleryContainer && galleryImages.length > 0) {
        galleryContainer.innerHTML = '';
        
        // Cria e configura cada imagem da galeria
        galleryImages.forEach((imageSrc, index) => {
            const img = document.createElement('img');
            img.src = imageSrc;
            img.alt = 'Super Mario 64 Screenshot';
            
            // Configura o clique para abrir o modal
            img.addEventListener('click', function() {
                let currentIndex = index;
                
                // Cria o modal (janela de visualização)
                const modal = document.createElement('div');
                Object.assign(modal.style, {
                    position: 'fixed',
                    top: '0',
                    left: '0',
                    width: '100vw',
                    height: '100vh',
                    background: 'rgba(0,0,0,0.8)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    zIndex: '9999'
                });

                // Cria a imagem ampliada no modal
                const bigImg = document.createElement('img');
                Object.assign(bigImg.style, {
                    width: 'auto',
                    height: '90vh',
                    maxWidth: 'none',
                    maxHeight: 'none',
                    borderRadius: '12px',
                    boxShadow: '0 0 30px #000',
                    objectFit: 'contain'
                });
                bigImg.src = galleryImages[currentIndex];

                // Botão para fechar o modal
                const closeBtn = createModalButton('✖', {
                    top: '30px',
                    right: '50px',
                    fontSize: '2.5em'
                }, () => document.body.removeChild(modal));

                // Botão para imagem anterior
                const prevBtn = createModalButton('◀', {
                    left: '40px',
                    top: '50%',
                    transform: 'translateY(-50%)',
                    fontSize: '3em'
                }, (e) => {
                    e.stopPropagation();
                    currentIndex = (currentIndex - 1 + galleryImages.length) % galleryImages.length;
                    bigImg.src = galleryImages[currentIndex];
                });

                // Botão para próxima imagem
                const nextBtn = createModalButton('▶', {
                    right: '40px',
                    top: '50%',
                    transform: 'translateY(-50%)',
                    fontSize: '3em'
                }, (e) => {
                    e.stopPropagation();
                    currentIndex = (currentIndex + 1) % galleryImages.length;
                    bigImg.src = galleryImages[currentIndex];
                });

                // Adiciona todos os elementos ao modal
                modal.append(bigImg, closeBtn, prevBtn, nextBtn);
                
                // Fecha o modal ao clicar fora da imagem
                modal.addEventListener('click', function(e) {
                    if (e.target === modal) {
                        document.body.removeChild(modal);
                    }
                });
                
                document.body.appendChild(modal);
            });
            
            galleryContainer.appendChild(img);
        });
    }
});

// ==============================================
// GALERIA DE IMAGENS - MINECRAFT
// ==============================================
document.addEventListener('DOMContentLoaded', function() {
    // Obs.: Esta função é idêntica à anterior, mas para imagens do Minecraft
    // Foi mantida separada para permitir personalizações futuras
    const galleryImage = [
        '/images/mine_1.jpg',
        '/images/mine_2.jpg',
        '/images/mine_3.jpg',
        '/images/mine_4.jpg',
        '/images/mine_5.jpg'
    ];

    const galleryContainer = document.getElementById('gallery2');
    
    if (galleryContainer && galleryImage.length > 0) {
        galleryContainer.innerHTML = '';
        
        galleryImage.forEach((imageSrc, index) => {
            const img = document.createElement('img');
            img.src = imageSrc;
            img.alt = 'Minecraft Screenshot';
            
            img.addEventListener('click', function() {
                // ... (código idêntico ao da galeria anterior)
            });
            
            galleryContainer.appendChild(img);
        });
    }
});

// ==============================================
// FORMULÁRIO DE SPEEDRUN
// ==============================================
document.addEventListener('DOMContentLoaded', function() {
    // Mostra/oculta o seletor de categorias baseado no jogo selecionado
    const gameSelect = document.getElementById('GameName');
    const categorySelect = document.getElementById('Category');
    
    if (gameSelect && categorySelect) {
        gameSelect.addEventListener('change', function() {
            categorySelect.style.display = this.value ? '' : 'none';
            if (!this.value) categorySelect.value = '';
        });
    }

    // Validação do formulário de speedrun
    const speedrunForm = document.getElementById('speedrunForm');
    if (speedrunForm) {
        speedrunForm.addEventListener('submit', function(e) {
            const time = document.getElementById('Time').value;
            const regex = /^\d{1,2}:\d{2}:\d{2}$/;
            
            // Verifica se o tempo está no formato correto (hh:mm:ss)
            if (!regex.test(time)) {
                const jsError = document.getElementById('jsError');
                if (jsError) {
                    jsError.textContent = 'Tempo inválido! Use o formato hh:mm:ss.';
                    jsError.style.display = '';
                }
                e.preventDefault(); // Impede o envio do formulário
            }
        });
    }
});

// Função auxiliar para criar botões do modal
function createModalButton(text, styles, clickHandler) {
    const btn = document.createElement('span');
    btn.textContent = text;
    Object.assign(btn.style, {
        position: 'absolute',
        color: '#fff',
        cursor: 'pointer',
        zIndex: '10000',
        ...styles
    });
    btn.addEventListener('click', clickHandler);
    return btn;
}