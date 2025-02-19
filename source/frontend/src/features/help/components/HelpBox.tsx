import './HelpBox.scss';

import { Button } from 'components/common/buttons/Button';
import React from 'react';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import styled from 'styled-components';

import { getTopics } from '../constants/HelpText';
import { IHelpPage, Topics } from '../interfaces';

interface IHelpBoxProps {
  /** The current help page that is being displayed */
  helpPage: IHelpPage | undefined;
  /** The active topic that is being displayed on this help page, only one topic is displayed at a time. */
  activeTopic: Topics;
  /** Set the active topic */
  setActiveTopic: Function;
}

const Box = styled.div`
  display: flex;
  align-items: start;
`;

const TopicButton = styled(Button)`
  max-height: 4rem;
  background-color: white;
  display: flex;
`;

const TopicSelector = styled(ButtonGroup)``;

/**
 * Display a list of topics, as well as the content component corresponding to the active topic.
 */
const HelpBox: React.FunctionComponent<React.PropsWithChildren<IHelpBoxProps>> = ({
  helpPage,
  activeTopic,
  setActiveTopic,
}) => {
  return (
    <Box className="help-box">
      <TopicSelector vertical className="col-md-4">
        {getTopics(helpPage).map((topic, index) => (
          <TopicButton
            key={`topics-${topic}-${index}`}
            onClick={() => setActiveTopic(topic)}
            active={topic === activeTopic}
            bsPrefix="link"
          >
            {topic}
          </TopicButton>
        ))}
      </TopicSelector>
      <span className="col-md-8">{helpPage?.topics.get(activeTopic)}</span>
    </Box>
  );
};

export default HelpBox;
