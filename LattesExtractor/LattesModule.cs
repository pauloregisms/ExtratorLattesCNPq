﻿using System;
using System.Collections.Generic;
using LattesExtractor.Controller;
using log4net;
using LattesExtractor.Entities;

namespace LattesExtractor
{
    class LattesModule
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(LattesModule).Name);

        protected CurriculoLattesWebService.WSCurriculoClient wscc;

        private static readonly object locker = new object();

        private Stack<CurriculoEntry> _curriculumVitaesForProcess = new Stack<CurriculoEntry>();
        private Stack<CurriculoEntry> _curriculumVitaeNumbersToDownload = new Stack<CurriculoEntry>();

        private String _tempDir = "";
        private String _qualisFile = "";
        private String _jcrFile = "";
        private string _lattesCurriculumValueQuery = null;
        private string _lattesCurriculumValueConnection = null;

        private static LattesModule _instance;

        public static LattesModule GetInstance()
        {
            if (_instance == null)
                _instance = new LattesModule();

            return _instance;
        }

        private LattesModule()
        {
            this.wscc = new CurriculoLattesWebService.WSCurriculoClient();
        }

        public String TempDirectory
        {
            get { return this._tempDir; }
            set { this._tempDir = value; }
        }

        public String QualisFileName
        {
            get { return this._qualisFile; }
            set { this._qualisFile = value; }
        }
        public string JCRFileName {
            get { return this._jcrFile; }
            set { this._jcrFile = value; } }

        public string LattesCurriculumVitaeQuery
        {
            get { return this._lattesCurriculumValueQuery; }
            set { this._lattesCurriculumValueQuery = value; }
        }


        public string LattesCurriculumVitaeODBCConnection
        {
            get { return this._lattesCurriculumValueConnection; }
            set { this._lattesCurriculumValueConnection = value; }
        }


        public CurriculoLattesWebService.WSCurriculoClient WSCurriculoClient { get { return this.wscc; } }

        public void Extract()
        {
            try {
                Logger.Info("Começando Processamento...");

                Logger.Info("Iniciando Carga dos Números de Currículo da Instituição...");
                LoadCurriculumVitaeNumberController.LoadCurriculumVitaeNumbers(this);

                Logger.Info("Iniciando Download dos Currículos Atualizados...");
                DownloadCurriculumVitaeController.DownloadUpdatedCurriculums(this);

                Logger.Info("Iniciando Processamento dos Currículos...");
                CurriculumVitaeProcessorController.ProcessCurriculumVitaes(this);
            }
            catch (Exception ex)
            {
                    var e1 = ex;
                    Logger.Error("Erros durante a execução:");
                    Logger.Error(ex.Message);
                
                    Logger.Error(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Logger.Error("Excessão Interna:");
                        int sequencia = 1;
                        while (ex.InnerException != null)
                        {
                            Logger.Error(String.Format("Excessão Interna [{0}]: {1}", sequencia++, ex.InnerException.Message));
                            Logger.Error(ex.StackTrace);
                            ex = ex.InnerException;
                        }
                    }
            }

            Logger.Info("Encerrando Execução...");
        }

        public void UpdateQualisDataBase (string csvQualis)
        {
            QualisFileName = csvQualis;
            Logger.Info("Iniciando Processamento do Qualis...");
            LoadQualisTableController.LoadQualisTable(this);
        }

        public void UpdateJCRImpactFactorDataBase(string inputJCRFile)
        {
            JCRFileName = inputJCRFile;
            Logger.Info("Iniciando Processamento do JCR Impact Factor...");
            LoadJCRTableController.LoadJCRTable(this);
        }

        public string GetCurriculumVitaeFileName(string curriculumVitaeNumber)
        {
            return String.Format("{0}\\{1}.xml", this.TempDirectory, curriculumVitaeNumber);
        }

        /// <summary>
        /// Adiciona um curriculo para curriculumVitaeUnserializer verificado para download
        /// </summary>
        /// <param name="curriculumVitaeNumber"></param>
        public void AddCurriculumVitaeNumberToDownload (CurriculoEntry curriculumVitaeNumber)
        {
            _curriculumVitaeNumbersToDownload.Push(curriculumVitaeNumber);
        }

        /// <summary>
        /// Retorna eu ainda existem curriculos para download
        /// </summary>
        public bool HasNextCurriculumVitaeNumberToDownload
        {
            get
            {
                return _curriculumVitaeNumbersToDownload.Count > 0;
            }
        }

        /// <summary>
        /// Remove e retorna o ultimo registro da pilha de curriculos pendente para download
        /// </summary>
        /// <returns></returns>
        public CurriculoEntry GetNextCurriculumVitaeNumberToDownload()
        {
            lock (locker)
            {
                if (_curriculumVitaeNumbersToDownload.Count > 0)
                    return _curriculumVitaeNumbersToDownload.Pop();
            }

            return null;
        }

        /// <summary>
        /// Adiciona um curriculo para curriculumVitaeUnserializer processado
        /// </summary>
        /// <param name="curriculumVitaeNumber"></param>
        public void AddCurriculumVitaeForProcess(CurriculoEntry curriculumVitaeNumber)
        {
            _curriculumVitaesForProcess.Push(curriculumVitaeNumber);
        }

        /// <summary>
        /// Retorna eu ainda existem curriculos para processar
        /// </summary>
        public bool HasNextCurriculumVitaeForProcess
        {
            get
            {
                return _curriculumVitaesForProcess.Count > 0;
            }
        }

        /// <summary>
        /// Remove e retorna o ultimo registro da pilha de curriculos pendente para processamento
        /// </summary>
        /// <returns></returns>
        public CurriculoEntry GetNextCurriculumVitaeForProcess()
        {
            lock (locker)
            {
                if (_curriculumVitaesForProcess.Count > 0)
                    return _curriculumVitaesForProcess.Pop();
            }

            return null;
        }
    }

}