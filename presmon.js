const userList = $('.p-channel_sidebar__channel--user');

   const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
       const presenceIndicator = $(mutation.target).find('.c-presence__indicator');
      if (presenceIndicator.length > 0) {
         const userId = presenceIndicator.closest('[data-member-id]').attr('data-member-id');
        const presence = presenceIndicator.attr('data-presence-status');
          console.log(`User ${userId} presence changed to ${presence}`);
      }
    });
  });

   observer.observe(userList[0], { childList: true, subtree: true });
