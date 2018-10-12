FROM debian:jessie-slim

RUN set -x \
    && apt-get update \
    && apt-get install -y exim4-daemon-light \
    && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* \
    && apt-get clean \
    && set +x \
    && find /var/log -type f | while read f; do echo -ne '' > $f; done;

COPY docker-entrypoint.sh /bin/entrypoint.sh
COPY set-exim4-update-conf.conf /bin/set-exim4-update-conf

RUN chmod a+x /bin/entrypoint.sh \
    && chmod a+x /bin/set-exim4-update-conf

EXPOSE 25
ENTRYPOINT ["/bin/entrypoint.sh"]
CMD ["exim", "-bd", "-q15m", "-v"]
