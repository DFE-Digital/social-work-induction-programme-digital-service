import styles from './PhaseBanner.module.scss';

function PhaseBanner() {
  return (
    <div class={styles['govuk-phase-banner']}>
      <p class={styles['govuk-phase-banner__content']}>
        <strong
          class={`${styles['govuk-tag']} ${styles['govuk-phase-banner__content__tag']}`}
        >
          Alpha
        </strong>
        <span class={styles['govuk-phase-banner__text']}>
          This is a new service - your{' '}
          <a class={styles['govuk-link']} href="#">
            feedback
          </a>{' '}
          will help us to improve it.
        </span>
      </p>
    </div>
  );
}

export default PhaseBanner;
