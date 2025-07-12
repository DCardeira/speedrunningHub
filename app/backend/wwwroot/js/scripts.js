document.addEventListener('DOMContentLoaded', function() {
    // Obtém o elemento de input que será usado para filtrar a lista de jogos
    const gameFilterInput = document.getElementById('gameFilter');
    // Obtém o elemento que contém a lista de jogos
    const gameList = document.getElementById('gameList');
    // Cria um array dos elementos 'li' que representam os jogos na lista
    const games = Array.from(gameList.getElementsByTagName('li'));

    // Adiciona um listener ao input que é usado sempre que o valor do input é alterado
    gameFilterInput.addEventListener('input', function() {
        // Obtém o valor do input e converte para minúsculas para facilitar a comparação
        const filterValue = gameFilterInput.value.toLowerCase();
        // Itera sobre cada jogo na lista
        games.forEach(function(game) {
            // Obtém o texto do jogo e converte para minúsculas
            const gameText = game.textContent.toLowerCase();
            // Verifica se o texto do jogo inclui o valor do filtro
            if (gameText.includes(filterValue)) {
                // Se incluir, define o estilo 'display' do jogo como padrão ('') para garantir que ele esteja visível
                game.style.display = '';
            } else {
                // Se não incluir, define o estilo 'display' do jogo como 'none' para escondê-lo
                game.style.display = 'none';
            }
        });
    });
});

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
        galleryImages.forEach((imageSrc, index) => {
            const img = document.createElement('img');
            img.src = imageSrc;
            img.alt = 'Super Mario 64 Screenshot';
            img.addEventListener('click', function() {
                let currentIndex = index;
                // Cria o modal
                const modal = document.createElement('div');
                modal.style.position = 'fixed';
                modal.style.top = '0';
                modal.style.left = '0';
                modal.style.width = '100vw';
                modal.style.height = '100vh';
                modal.style.background = 'rgba(0,0,0,0.8)';
                modal.style.display = 'flex';
                modal.style.alignItems = 'center';
                modal.style.justifyContent = 'center';
                modal.style.zIndex = '9999';

                const bigImg = document.createElement('img');
                bigImg.src = galleryImages[currentIndex];
                bigImg.style.width = 'auto';
                bigImg.style.height = '90vh';
                bigImg.style.maxWidth = 'none';
                bigImg.style.maxHeight = 'none';
                bigImg.style.borderRadius = '12px';
                bigImg.style.boxShadow = '0 0 30px #000';
                bigImg.style.objectFit = 'contain';

                // Botão fechar
                const closeBtn = document.createElement('span');
                closeBtn.textContent = '✖';
                closeBtn.style.position = 'absolute';
                closeBtn.style.top = '30px';
                closeBtn.style.right = '50px';
                closeBtn.style.fontSize = '2.5em';
                closeBtn.style.color = '#fff';
                closeBtn.style.cursor = 'pointer';
                closeBtn.style.zIndex = '10000';
                closeBtn.addEventListener('click', function() {
                    document.body.removeChild(modal);
                });

                // Botão anterior
                const prevBtn = document.createElement('span');
                prevBtn.textContent = '◀';
                prevBtn.style.position = 'absolute';
                prevBtn.style.left = '40px';
                prevBtn.style.top = '50%';
                prevBtn.style.transform = 'translateY(-50%)';
                prevBtn.style.fontSize = '3em';
                prevBtn.style.color = '#fff';
                prevBtn.style.cursor = 'pointer';
                prevBtn.style.zIndex = '10000';
                prevBtn.addEventListener('click', function(e) {
                    e.stopPropagation();
                    currentIndex = (currentIndex - 1 + galleryImages.length) % galleryImages.length;
                    bigImg.src = galleryImages[currentIndex];
                });

                // Botão seguinte
                const nextBtn = document.createElement('span');
                nextBtn.textContent = '▶';
                nextBtn.style.position = 'absolute';
                nextBtn.style.right = '40px';
                nextBtn.style.top = '50%';
                nextBtn.style.transform = 'translateY(-50%)';
                nextBtn.style.fontSize = '3em';
                nextBtn.style.color = '#fff';
                nextBtn.style.cursor = 'pointer';
                nextBtn.style.zIndex = '10000';
                nextBtn.addEventListener('click', function(e) {
                    e.stopPropagation();
                    currentIndex = (currentIndex + 1) % galleryImages.length;
                    bigImg.src = galleryImages[currentIndex];
                });

                modal.appendChild(bigImg);
                modal.appendChild(closeBtn);
                modal.appendChild(prevBtn);
                modal.appendChild(nextBtn);
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
})

document.addEventListener('DOMContentLoaded', function() {
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
                let currentIndex = index;
                // Cria o modal
                const modal = document.createElement('div');
                modal.style.position = 'fixed';
                modal.style.top = '0';
                modal.style.left = '0';
                modal.style.width = '100vw';
                modal.style.height = '100vh';
                modal.style.background = 'rgba(0,0,0,0.8)';
                modal.style.display = 'flex';
                modal.style.alignItems = 'center';
                modal.style.justifyContent = 'center';
                modal.style.zIndex = '9999';

                const bigImg = document.createElement('img');
                bigImg.src = galleryImage[currentIndex];
                bigImg.style.width = 'auto';
                bigImg.style.height = '90vh';
                bigImg.style.maxWidth = 'none';
                bigImg.style.maxHeight = 'none';
                bigImg.style.borderRadius = '12px';
                bigImg.style.boxShadow = '0 0 30px #000';
                bigImg.style.objectFit = 'contain';

                // Botão fechar
                const closeBtn = document.createElement('span');
                closeBtn.textContent = '✖';
                closeBtn.style.position = 'absolute';
                closeBtn.style.top = '30px';
                closeBtn.style.right = '50px';
                closeBtn.style.fontSize = '2.5em';
                closeBtn.style.color = '#fff';
                closeBtn.style.cursor = 'pointer';
                closeBtn.style.zIndex = '10000';
                closeBtn.addEventListener('click', function() {
                    document.body.removeChild(modal);
                });

                // Botão anterior
                const prevBtn = document.createElement('span');
                prevBtn.textContent = '◀';
                prevBtn.style.position = 'absolute';
                prevBtn.style.left = '40px';
                prevBtn.style.top = '50%';
                prevBtn.style.transform = 'translateY(-50%)';
                prevBtn.style.fontSize = '3em';
                prevBtn.style.color = '#fff';
                prevBtn.style.cursor = 'pointer';
                prevBtn.style.zIndex = '10000';
                prevBtn.addEventListener('click', function(e) {
                    e.stopPropagation();
                    currentIndex = (currentIndex - 1 + galleryImage.length) % galleryImage.length;
                    bigImg.src = galleryImage[currentIndex];
                });

                // Botão seguinte
                const nextBtn = document.createElement('span');
                nextBtn.textContent = '▶';
                nextBtn.style.position = 'absolute';
                nextBtn.style.right = '40px';
                nextBtn.style.top = '50%';
                nextBtn.style.transform = 'translateY(-50%)';
                nextBtn.style.fontSize = '3em';
                nextBtn.style.color = '#fff';
                nextBtn.style.cursor = 'pointer';
                nextBtn.style.zIndex = '10000';
                nextBtn.addEventListener('click', function(e) {
                    e.stopPropagation();
                    currentIndex = (currentIndex + 1) % galleryImage.length;
                    bigImg.src = galleryImage[currentIndex];
                });

                modal.appendChild(bigImg);
                modal.appendChild(closeBtn);
                modal.appendChild(prevBtn);
                modal.appendChild(nextBtn);
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
})

document.addEventListener('DOMContentLoaded', function() {
    var gameSelect = document.getElementById('GameName');
    var categorySelect = document.getElementById('Category');
    if (gameSelect && categorySelect) {
        gameSelect.addEventListener('change', function() {
            if (this.value) {
                categorySelect.style.display = '';
            } else {
                categorySelect.style.display = 'none';
                categorySelect.value = '';
            }
        });
    }
    var speedrunForm = document.getElementById('speedrunForm');
    if (speedrunForm) {
        speedrunForm.addEventListener('submit', function(e) {
            var time = document.getElementById('Time').value;
            var regex = /^\d{1,2}:\d{2}:\d{2}$/;
            if (!regex.test(time)) {
                var jsError = document.getElementById('jsError');
                if (jsError) {
                    jsError.textContent = 'Tempo inválido! Use o formato hh:mm:ss.';
                    jsError.style.display = '';
                }
                e.preventDefault();
            }
        });
    }
});